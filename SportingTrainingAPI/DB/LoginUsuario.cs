//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SportingTrainingAPI.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class LoginUsuario
    {
        public int IdLoginUsuario { get; set; }
        public string IdUsuario { get; set; }
        public string Token { get; set; }
        public System.DateTime InicioSessao { get; set; }
        public System.DateTime FimSessao { get; set; }
    
        public virtual Usuario Usuario { get; set; }
    }
}