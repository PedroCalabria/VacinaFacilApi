﻿using log4net;
using VacinaFacil.Business.Interface.IBusinesses;
using VacinaFacil.Entity.DTO;
using VacinaFacil.Entity.Model;
using VacinaFacil.Repository.Interface.IRepositories;
using VacinaFacil.Utils.Exceptions;
using VacinaFacil.Utils.Messages;

namespace VacinaFacil.Business.Businesses
{
    public class AppointmentBusiness : IAppointmentBusiness
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(AppointmentBusiness));
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentBusiness(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<List<GroupedAppointmentDTO>> DeleteAppointment(int idAppointment)
        {
            var appointment = await _appointmentRepository.getByID(idAppointment);

            if (appointment == null)
            {
                _log.InfoFormat(BusinessMessages.RecordNotFound);
                throw new BusinessException(BusinessMessages.RecordNotFound);
            }

            await _appointmentRepository.Delete(appointment);

            _log.InfoFormat(BusinessMessages.SuccessfulOperation);
            return await _appointmentRepository.ListAll();

        }

        public async Task<List<GroupedAppointmentDTO>> InsertAppointment(InsertAppointmentModel appointment)
        {
            var appointmentAvailability = await CheckAppointmentAvailability(appointment.AppointmentDate, appointment.AppointmentTime);
            
            if (!appointmentAvailability)
            {
                _log.InfoFormat(string.Format(BusinessMessages.ExistingRecord, new { appointment.AppointmentDate, appointment.AppointmentTime }));
                throw new BusinessException(string.Format(BusinessMessages.ExistingRecord, new { appointment.AppointmentDate, appointment.AppointmentTime }));
            }

            await _appointmentRepository.InsertAppointment(appointment);

            _log.InfoFormat(BusinessMessages.SuccessfulOperation);
            return await _appointmentRepository.ListAll();
        }

        public async Task<List<GroupedAppointmentDTO>> ListAppointments()
        {
            return await _appointmentRepository.ListAll();

        }

        public async Task<List<GroupedAppointmentDTO>> ListAppointmentsByDate(DateTime date)
        {
            return await _appointmentRepository.ListByDate(date);
        }

        public async Task<List<GroupedAppointmentDTO>> UpdateAppointment(int idAppointment, UpdateAppointmentModel newAppointment)
        {
            var appointment = await _appointmentRepository.getByID(idAppointment);

            if (appointment == null)
            {
                _log.InfoFormat(BusinessMessages.RecordNotFound);
                throw new BusinessException(BusinessMessages.RecordNotFound);
            }

            var appointmentAvailability = await CheckAppointmentAvailability(newAppointment.AppointmentDate, newAppointment.AppointmentTime);

            if (!appointmentAvailability)
            {
                _log.InfoFormat(BusinessMessages.AppointmentNotAvailable);
                throw new BusinessException(BusinessMessages.AppointmentNotAvailable);
            }

            appointment.AppointmentDate = newAppointment.AppointmentDate;
            appointment.AppointmentTime = newAppointment.AppointmentTime;
            appointment.Scheduled = newAppointment.Scheduled;
            appointment.CriationDate = DateTime.Now;

            await _appointmentRepository.Update(appointment);

            _log.InfoFormat(BusinessMessages.SuccessfulOperation);
            return await _appointmentRepository.ListAll();
        }

        private async Task<bool> CheckAppointmentAvailability(DateTime date, TimeSpan time)
        {
            var appointments = await _appointmentRepository.ListByDate(date);
            var appointmentsDay = appointments.Sum(a => a.Count);
            var appointmentsTime = appointments.Find(a => a.AppointmentTime == time)?.Count ?? 0;

            if (appointmentsDay >= 20 || appointmentsTime >= 2)
            {
                return false;
            }

            return true;
        }
    }
}
