using AutoMapper;
using SecureOPS.Applications.DTOs;
using SecureOPS.Applications.RepositoryInterfaces;
using SecureOPS.Applications.ServiceInterfaces;
using SecureOPS.Applications.Exceptions; // Required for custom exceptions
using SecureOPS.Domain.Entities;

namespace SecureOPS.Applications.Services
{
    public class IncidentTaskService : IIncidentTaskService
    {
        private readonly IIncidentTaskRepository _taskRepo;
        private readonly INotificationService _notifService;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepo;

        public IncidentTaskService(IIncidentTaskRepository taskRepo, INotificationService notifService, IMapper mapper, IUserRepository userRepo)
        {
            _taskRepo = taskRepo;
            _notifService = notifService;
            _mapper = mapper;
            _userRepo = userRepo;
        }

        // 1. Creation & Assignment
        public async Task<IncidentTaskDto> CreateAndAssignTaskAsync(IncidentTaskDto dto)
        {
            if (dto == null)
                throw new AppValidationException("Task data cannot be null.");

            try
            {
                var task = _mapper.Map<IncidentTask>(dto);
                task.Status = "Pending";
                var result = await _taskRepo.AddAsync(task);

                // Targeted notification to the specific responder assigned
                await _notifService.CreateSystemNotificationAsync(
                    $"New Assignment: {task.Description} for Incident #{task.IncidentId}",
                    "Task-New",
                    task.AssignedTo);

                return _mapper.Map<IncidentTaskDto>(result);
            }
            catch (Exception ex)
            {
                throw new Exception("An internal error occurred while creating and assigning the task.", ex);
            }
        }

        // 2. Status Progression
        public async Task UpdateTaskStatusAsync(int taskId, string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new AppValidationException("Status update requires a valid value.");

            var task = await _taskRepo.GetByIdAsync(taskId);
            if (task == null)
                throw new NotFoundException($"Incident Task {taskId} not found.");

            try
            {
                task.Status = status;
                await _taskRepo.UpdateAsync(task);

                // Notify L2 Analyst when task is finished
                // UPDATED: Using full role name to match your DB cleanup
                if (status == "Completed")
                {
                    await _notifService.NotifyRoleAsync($"Task #{taskId} Completed", "Task-Done", "L2 Analyst");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update status for Task {taskId}.", ex);
            }
        }

        // 3. Work Documentation
        public async Task AddDocumentationAsync(int taskId, string notes)
        {
            if (string.IsNullOrWhiteSpace(notes))
                throw new AppValidationException("Documentation notes cannot be empty.");

            var task = await _taskRepo.GetByIdAsync(taskId);
            if (task == null)
                throw new NotFoundException($"Task {taskId} not found for documentation.");

            task.Description += $"\n[Update {DateTime.Now}]: {notes}";
            await _taskRepo.UpdateAsync(task);
        }

        // 4. Reassignment & Escalation
        public async Task ReassignTaskAsync(int taskId, int newUserId, bool isEscalation)
        {
            var task = await _taskRepo.GetByIdAsync(taskId);
            if (task == null)
                throw new NotFoundException($"Cannot reassign. Task {taskId} was not found.");

            // Verify the new user exists before notifying them
            var userExists = await _userRepo.GetByIdAsync(newUserId);
            if (userExists == null)
                throw new NotFoundException($"Reassignment failed: User {newUserId} does not exist.");

            try
            {
                task.AssignedTo = newUserId;
                await _taskRepo.UpdateAsync(task);

                string message = isEscalation
                    ? $"URGENT: Task #{taskId} has been escalated to you."
                    : $"Task #{taskId} has been reassigned to you.";

                // Targeted notification to the NEW user
                await _notifService.CreateSystemNotificationAsync(message, "Task-Reassign", newUserId);
            }
            catch (Exception ex)
            {
                throw new Exception($"System failure during task reassignment for Task {taskId}.", ex);
            }
        }

        public async Task<IncidentTaskDto> GetTaskByIdAsync(int taskId)
        {
            var task = await _taskRepo.GetByIdAsync(taskId);
            if (task == null)
                throw new NotFoundException($"Task {taskId} not found.");

            return _mapper.Map<IncidentTaskDto>(task);
        }
    }
}