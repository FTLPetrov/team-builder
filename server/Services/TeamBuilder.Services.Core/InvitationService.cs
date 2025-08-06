using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TeamBuilder.Data.Models;
using TeamBuilder.Services.Core.Contracts.Invitation.Requests;
using TeamBuilder.Services.Core.Contracts.Invitation.Responses;
using TeamBuilder.Services.Core.Interfaces;
using TeamBuilder.Data.Repositories;
using TeamBuilder.Data.Repositories.Interfaces;

namespace TeamBuilder.Services.Core
{
    public class InvitationService : IInvitationService
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly UserManager<User> _userManager;
        
        public InvitationService(IInvitationRepository invitationRepository, ITeamMemberRepository teamMemberRepository, UserManager<User> userManager)
        {
            _invitationRepository = invitationRepository;
            _teamMemberRepository = teamMemberRepository;
            _userManager = userManager;
        }

        public async Task<IEnumerable<InvitationResponse>> GetAllAsync(Guid teamId)
        {
            var invitations = await _invitationRepository.GetInvitationsByTeamAsync(teamId);
            return invitations.Select(MapToInvitationResponse);
        }

        public async Task<IEnumerable<InvitationResponse>> GetUserInvitationsAsync(Guid userId)
        {
            Console.WriteLine($"InvitationService: Looking for invitations for user {userId}");
            
            var invitations = await _invitationRepository.GetPendingInvitationsAsync(userId);
            
            Console.WriteLine($"InvitationService: Found {invitations.Count()} invitations in database");
            foreach (var invitation in invitations)
            {
                Console.WriteLine($"InvitationService: Invitation {invitation.Id} - Team: {invitation.Team?.Name}, InvitedBy: {invitation.InvitedBy?.UserName}");
            }
            
            return invitations.Select(MapToInvitationResponse);
        }

        public async Task<InvitationResponse?> GetByIdAsync(Guid invitationId)
        {
            var invitation = await _invitationRepository.GetByIdAsync(invitationId);
            return invitation == null ? null : MapToInvitationResponse(invitation);
        }

        public async Task<InvitationCreateResponse> CreateAsync(InvitationCreateRequest request)
        {
            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                TeamId = request.TeamId,
                InvitedUserId = request.InvitedUserId,
                InvitedById = request.InvitedById,
                SentAt = DateTime.UtcNow,
                Accepted = false
            };
            
            await _invitationRepository.AddAsync(invitation);
            
            return new InvitationCreateResponse
            {
                Success = true,
                Id = invitation.Id
            };
        }

        public async Task<InvitationCreateResponse> CreateByEmailAsync(InvitationCreateByEmailRequest request)
        {
            Console.WriteLine($"CreateByEmailAsync: Starting invitation creation");
            Console.WriteLine($"CreateByEmailAsync: TeamId = {request.TeamId}");
            Console.WriteLine($"CreateByEmailAsync: InvitedUserEmail = {request.InvitedUserEmail}");
            Console.WriteLine($"CreateByEmailAsync: InvitedById = {request.InvitedById}");
            

            var invitedUser = await _userManager.FindByEmailAsync(request.InvitedUserEmail);
            if (invitedUser == null)
            {
                Console.WriteLine($"CreateByEmailAsync: User not found for email {request.InvitedUserEmail}");
                return new InvitationCreateResponse
                {
                    Success = false,
                    ErrorMessage = $"User with email '{request.InvitedUserEmail}' not found. Please make sure the user is registered in the system."
                };
            }
            
            Console.WriteLine($"CreateByEmailAsync: Found user {invitedUser.Id} for email {request.InvitedUserEmail}");


            var existingInvitation = await _invitationRepository.FirstOrDefaultAsync(i => 
                i.TeamId == request.TeamId && 
                i.InvitedUserId == invitedUser.Id && 
                i.RespondedAt == null);
            
            if (existingInvitation != null)
            {
                return new InvitationCreateResponse
                {
                    Success = true,
                    Id = existingInvitation.Id,
                    Message = $"User has been already invited for team \"{existingInvitation.Team?.Name ?? "Unknown Team"}\"",
                    IsAlreadyInvited = true
                };
            }

            Console.WriteLine($"CreateByEmailAsync: Creating new invitation");
            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                TeamId = request.TeamId,
                InvitedUserId = invitedUser.Id,
                InvitedById = request.InvitedById,
                SentAt = DateTime.UtcNow,
                Accepted = false
            };
            
            await _invitationRepository.AddAsync(invitation);
            
            Console.WriteLine($"CreateByEmailAsync: Invitation created successfully with ID {invitation.Id}");
            return new InvitationCreateResponse
            {
                Success = true,
                Id = invitation.Id
            };
        }

        public async Task<InvitationRespondResponse> RespondAsync(InvitationRespondRequest request)
        {
            var invitation = await _invitationRepository.GetByIdWithTeamAsync(request.InvitationId);
                
            if (invitation == null)
                return new InvitationRespondResponse { Success = false, ErrorMessage = "Invitation not found" };
            if (invitation.RespondedAt != null)
                return new InvitationRespondResponse { Success = false, ErrorMessage = "Invitation already responded to" };
            

            if (!request.Accept)
            {
                await _invitationRepository.DeleteAsync(invitation);
                
                return new InvitationRespondResponse
                {
                    Success = true,
                    Id = invitation.Id,
                    Accepted = false,
                    TeamName = invitation.Team?.Name ?? "Unknown Team"
                };
            }
            

            invitation.Accepted = true;
            invitation.RespondedAt = DateTime.UtcNow;
            

            var existingMember = await _teamMemberRepository.GetByTeamAndUserAsync(invitation.TeamId, invitation.InvitedUserId);
                
            if (existingMember == null)
            {
                var teamMember = new TeamMember
                {
                    TeamId = invitation.TeamId,
                    UserId = invitation.InvitedUserId,
                    Role = TeamRole.Member
                };
                await _teamMemberRepository.AddAsync(teamMember);
            }
            
            await _invitationRepository.UpdateAsync(invitation);
            
            return new InvitationRespondResponse
            {
                Success = true,
                Id = invitation.Id,
                Accepted = true,
                TeamName = invitation.Team?.Name ?? "Unknown Team"
            };
        }

        public async Task<bool> DeleteAsync(Guid invitationId)
        {
            var invitation = await _invitationRepository.GetByIdAsync(invitationId);
            if (invitation == null) return false;
            
            await _invitationRepository.DeleteAsync(invitation);
            return true;
        }

        private static InvitationResponse MapToInvitationResponse(Invitation invitation)
        {
            return new InvitationResponse
            {
                Id = invitation.Id,
                TeamId = invitation.TeamId,
                InvitedUserId = invitation.InvitedUserId,
                InvitedById = invitation.InvitedById,
                SentAt = invitation.SentAt,
                Accepted = invitation.Accepted,
                RespondedAt = invitation.RespondedAt,
                TeamName = invitation.Team?.Name ?? "Unknown Team",
                SenderName = invitation.InvitedBy?.UserName ?? invitation.InvitedBy?.Email ?? "Unknown Sender",
                SenderEmail = invitation.InvitedBy?.Email ?? "",
                InvitedUserName = invitation.InvitedUser?.UserName ?? invitation.InvitedUser?.Email ?? "Unknown User",
                InvitedUserEmail = invitation.InvitedUser?.Email ?? ""
            };
        }
    }
} 