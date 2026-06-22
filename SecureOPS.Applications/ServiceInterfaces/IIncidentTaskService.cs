using SecureOPS.Applications.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOPS.Applications.ServiceInterfaces
{
    public interface IIncidentTaskService
    {
        // 1. Creation & Assignment
        Task<IncidentTaskDto> CreateAndAssignTaskAsync(IncidentTaskDto dto);

        // 2. Status Progression
        Task UpdateTaskStatusAsync(int taskId, string status);

        // 3. Work Documentation
        Task AddDocumentationAsync(int taskId, string notes);

        // 4. Reassignment & Escalation
        Task ReassignTaskAsync(int taskId, int newUserId, bool isEscalation);

        Task<IncidentTaskDto> GetTaskByIdAsync(int taskId);
    }
}
