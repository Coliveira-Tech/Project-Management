﻿using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Domain.Dtos
{
    public class ProjectDto
    {
        public ProjectDto() { }
        public ProjectDto(Project entity)
        {
            if (entity == null)
                return;

            Id = entity.Id;
            Name = entity.Name;
            Description = entity.Description;
            Owner = new UserDto(entity.Owner);

            entity.Tasks.ForEach(task =>
            {
                Tasks.Add(new (task));
            });
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public UserDto Owner { get; set; } = null!;
        public List<TaskDto> Tasks { get; set; } = [];
    }
}
