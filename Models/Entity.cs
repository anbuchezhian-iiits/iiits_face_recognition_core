using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iiits_face_recognition_core.Models
{
    // Person table model
    public class Person
    {
        public int Id { get; set; }
        public string PersonName { get; set; }
        public string Photo1Path { get; set; }
        public string Photo2Path { get; set; }
        [Column(TypeName = "bytea")]
        public byte[] Embedding { get; set; }
    }

    // RecognizedPerson table model

    public class RecognizedPerson
    {
        public int ID { get; set; }
        public string RecognizedPersonName { get; set; }
        public string CapturedPhotoPath { get; set; }
        public byte[] UnknownEmbedding { get; set; }
        public double SimilarityScore { get; set; }
        public DateTime EntryDateTime { get; set; }
    }

}
