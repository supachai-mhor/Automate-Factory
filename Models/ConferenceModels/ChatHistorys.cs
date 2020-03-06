using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AutomateBussiness.Models.ConferenceModels
{  
    public enum MessageType
        {
            text,
            file,
            image,
            video,
            link
        }
    public enum MessageStatus
    {
        unread,
        read
    }
    public class ChatHistorys
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string id { get; set; }
        [Required]
        public DateTime messageDate { get; set; }

        [Required]
        public string senderId { get; set; }

        [Required]
        public string receiverId { get; set; }

        [Required]
        public string message { get; set; }

        [Required]
        public MessageType messageType { get; set; }

        [Required]
        public MessageStatus messageStatus { get; set; }

    }
}
