using Instances.Application.Trainings;
using Instances.Application.Trainings.Restoration;
using Instances.Domain.Trainings;
using Instances.Web.Controllers.Dtos;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System;
using System.Threading.Tasks;

namespace Instances.Web.Controllers
{
    [ApiController, Route("/api/trainings")]
    [ApiSort("-" + nameof(Training.Id))]
    public class TrainingsController
    {
        private readonly TrainingsRepository _trainingsRepository;
        private readonly TrainingRestorer _restorer;
        private readonly ITrainingRestorationCompleter _restorationCompleter;

        public TrainingsController
        (
            TrainingsRepository trainingsRepository,
            TrainingRestorer restorer,
            ITrainingRestorationCompleter restorationCompleter
        )
        {
            _trainingsRepository = trainingsRepository;
            _restorer = restorer;
            _restorationCompleter = restorationCompleter;
        }

        [HttpGet]
        [ForbidIfMissing(Operation.ReadEnvironments)]
        public Task<Page<Training>> GetAsync([FromQuery]TrainingListQuery query)
        {
            return _trainingsRepository.GetTrainingsAsync(query.Page, query.ToTrainingFilter());
        }

        [HttpPost("restore")]
        [ForbidIfMissing(Operation.RestoreInstances)]
        public Task<TrainingRestoration> Duplicate(TrainingRestorationRequest request)
        {
            return _restorer.CreateRestorationAsync(request);
        }

        [HttpPost("restorations/{restorationId:guid}/notify")]
        [ForbidIfMissing(Operation.RestoreInstances)]
        public Task RestorationReport
        (
            [FromRoute]Guid restorationId,
            [FromBody]TrainingRestorationCallbackPayload payload
        )
        {
            return _restorationCompleter.MarkRestorationAsCompletedAsync(restorationId, payload.Success);
        }
    }
}
