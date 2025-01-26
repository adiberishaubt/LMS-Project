using System.Collections.Generic;
using System.Linq;

namespace Library_Managment_System.Models.Common
{
    public class Result
    {
        private readonly List<string> _errors;
        private readonly bool _success;

        public Result(IEnumerable<string> errors, bool success)
        {
            _errors = errors.ToList();
            _success = success;
        }

        public List<string> Errors => _errors;

        public bool Success => _success;

        public static Result Failure(IEnumerable<string> errors) => new Result(errors, false); 
        public static Result Failure(string errors) => new Result(new List<string> { errors }, false);
        public static Result Succed() => new Result(new List<string>(), true);

    }
}
