using app.data;
using app.domain.Exceptions;
using app.domain.Model.Entities;
using app.service.Model.Response;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace app.service
{
    public class OptionService : IOptionService
    {
        private readonly IEntityRepository _entityRepository;
        private readonly ILogger<AccountService> _logger;

        public OptionService(ISecurityService securityService,
            IEntityRepository entityRepository,
            ILogger<AccountService> logger
            )
        {
            _entityRepository = entityRepository;
            _logger = logger;
        }

        public GenericServiceResponse<Option> GetOrCreate(string sec)
        {
            var response = new GenericServiceResponse<Option>();
            try
            {
                Option model = null;
                model = _entityRepository.GetEntityBy<Option>(new Dictionary<string, object> { { "Sec", sec } });
                if (model == null)
                {
                    model = new Option
                    {
                        Sec = sec,
                        NameAZ = string.Empty,
                        NameEN = string.Empty,
                        NameRU = string.Empty
                    };

                    int id = _entityRepository.Create<Option>(model, "", "", false);
                    if (id < 1)
                        throw new BusinessException("Error on Option create: " + sec);

                    model.Id = id;
                }

                response.Model = model;
                response.IsSuccessfull = true;
            }
            catch (BusinessException exp)
            {
                response.LoadFrom(exp);
            }
            catch (Exception exp)
            {
                response.ErrorMessage = exp.ToString();
            }
            return response;
        }
    }
}
