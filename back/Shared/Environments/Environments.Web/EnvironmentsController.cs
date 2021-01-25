using Environments.Application;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Environment = Environments.Domain.Environment;

namespace Environments.Web
{
    [ApiController, Route("api/environments")]

    public class EnvironmentsController
    {
        private readonly EnvironmentsRepository _repository;

        public EnvironmentsController(EnvironmentsRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public List<Environment> Get()
        {
            return _repository.Get();
        }
    }
}
