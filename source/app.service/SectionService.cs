using app.data;
using app.domain.Exceptions;
using app.domain.Model.Entities;
using app.service.Model.Response;
using app.service.Validations;
using System;

namespace app.service
{
    public class SectionService : ISectionService
    {
        private readonly IEntityRepository _entityRepository;

        public SectionService(IEntityRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        public GenericServiceResponse<Section> Create(int courseId, string name, User currrentUser)
        {
            var response = new GenericServiceResponse<Section>();
            try
            {
                Validator.SectionCreate(courseId, name);

                //db validations
                Course course = _entityRepository.GetEntityById<Course>(courseId);
                if (course == null)
                {
                    throw new BusinessException("Section should be related to any Course");
                }
                if (course.UserId != currrentUser.Id)
                {
                    throw new BusinessException("You can add Section only to own Course");
                }

                Section model = new Section
                {
                    CourseId = courseId,
                    Name = name
                };

                int entityId = _entityRepository.Create<Section>(model, "Name", name, false);
                if (entityId > 0) //entity save is ok
                {
                    model.Id = entityId;
                }
                else
                {
                    throw new BusinessException("Error on course creation");
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
