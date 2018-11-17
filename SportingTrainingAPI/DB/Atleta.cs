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
    
    public partial class Atleta
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Atleta()
        {
            this.EventosAtleta = new HashSet<EventoAtleta>();
        }
    
        public string IdAtleta { get; set; }
        public string IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public System.DateTime DataNascimento { get; set; }
        public string TipoSanguineo { get; set; }
        public string PlanoDeSaude { get; set; }
        public string Cpf { get; set; }
        public string Nacionalidade { get; set; }
        public string Equipe { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EventoAtleta> EventosAtleta { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}