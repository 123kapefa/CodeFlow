using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionService.Domain.Filters;

public record PageParams (int? Page, int? PageSize);
