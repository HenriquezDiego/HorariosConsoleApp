﻿using HorariosConsoleApp.Entities;
using HorariosConsoleApp.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace HorariosConsoleApp.Services
{
    public class PagoEmpleadoService : IPagoEmpleadoService
    {
        private readonly AppDbContext _dbContext;

        public PagoEmpleadoService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Calcular(DateTime fechaInicio, DateTime fechaFin)
        {
            var fecha = fechaInicio;

            var empleados = _dbContext.Empleados
                            .Include(em=>em.Equipo)
                            .ThenInclude(em=>em.Horario).ToList();


            var horasDetalle = _dbContext.ConsultaHoraDetalle.ToList();
            
            foreach (var empleado in empleados)
            {
                string horario;

                while (fecha<=fechaFin)
                {
                    var dia = fecha.DayOfWeek == 0 ? 7 : (int) fecha.DayOfWeek;
                            
                    var pagoEmpleado = new PagoEmpleado()
                    {
                        EmpleadoId = empleado.EmpleadoId,
                        Equipo = empleado.Equipo.Nombre,
                        Fecha = fecha,
                        SalarioBase = empleado.SalarioBase
                    };

                    horario = empleado.Equipo.Horario.Alias;
                    //if (logCambioHorarios.Any())
                    //{ 
                    //    var cambio = logCambioHorarios
                    //        .FirstOrDefault(log => log.EmpleadoId == empleado.EmpleadoId                            &&fechaInicio>=log.FechaInicio&&fechaInicio<=log.FechaFin);
                    //    horario = cambio.Horario;
                    //}

                    pagoEmpleado.Horario = horario;
                            
                    var horasDia = horasDetalle.Where(hd => hd.Horario.Equals(horario)
                                                            &&hd.DiaId==dia).ToList();


                    foreach (var detalle in horasDia)
                    {
                        //pagoEmpleado.DetallePago.Add(new DetallePagoEmpleado()
                        //{
                        //    TipoHora = detalle.TipoHora,
                        //    CantidadHoras = detalle.NumeroHoras,
                        //    Porcentaje = detalle.PorcentajeHora,
                        //    EsNocturna = detalle.EsNocturna
                        //});
                    }

                    _dbContext.PagoEmpleados.Add(pagoEmpleado);
                    fecha = fecha.AddDays(1);
                }

                fecha = fechaInicio;
            }

            _dbContext.SaveChanges();
        }

    }
}
