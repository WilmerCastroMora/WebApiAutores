﻿namespace WebApiAutores.DTOs
{
    public class RespuestaAutenticacionDTO
    {
        public string Token { get; set; }
        public DateTime FechaVencimiento { get; set; }
    }
}
