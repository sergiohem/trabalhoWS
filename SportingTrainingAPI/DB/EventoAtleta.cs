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
    
    public partial class EventoAtleta
    {
        public int IdEventoAtleta { get; set; }
        public int IdEvento { get; set; }
        public string IdAtleta { get; set; }
        public string InscricaoAtleta { get; set; }
        public System.DateTime DataInscricao { get; set; }
    
        public virtual Atleta Atleta { get; set; }
        public virtual Evento Evento { get; set; }
    }
}
