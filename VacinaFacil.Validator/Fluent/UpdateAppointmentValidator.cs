﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacinaFacil.Entity.Enum;
using VacinaFacil.Entity.Model;
using VacinaFacil.Utils.Messages;

namespace VacinaFacil.Validator.Fluent
{
    public class UpdateAppointmentValidator : AbstractValidator<UpdateAppointmentModel>
    {
        public UpdateAppointmentValidator()
        {
            RuleFor(appointment => appointment.AppointmentDate)
                .NotEmpty().WithMessage(string.Format(BusinessMessages.MandatoryField, "AppointmentDate"))
                .GreaterThanOrEqualTo(DateTime.Now.Date).WithMessage(string.Format(BusinessMessages.MinDate, "AppointmentDate", DateTime.Now.Date));

            RuleFor(appointment => appointment.AppointmentTime)
                .NotEmpty().WithMessage(string.Format(BusinessMessages.MandatoryField, "AppointmentTime"));

            RuleFor(appointment => appointment.Scheduled)
                .NotEmpty().WithMessage(string.Format(BusinessMessages.MandatoryField, "Scheduled"))
                .IsInEnum().WithMessage(string.Format(BusinessMessages.EnumOutOfRange, "Scheduled"));
        }
    }
}
