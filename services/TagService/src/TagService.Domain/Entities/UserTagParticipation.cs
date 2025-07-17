using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagService.Domain.Entities;

public class UserTagParticipation {
    public Guid Id { get; set; }

    public Guid UserId { get; set; }     
    public int QuestionsCreated { get; set; }  // количество вопросов с тегом    
    public int AnswersWritten { get; set; } // количество ответов с тегом
    public DateTime LastActiveAt { get; set; }

    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
