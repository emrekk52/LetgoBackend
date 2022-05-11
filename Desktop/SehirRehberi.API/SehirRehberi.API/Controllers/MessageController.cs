using LetgoEcommerce.Data;
using LetgoEcommerce.Dtos;
using LetgoEcommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetgoEcommerce.Controllers
{

    [Produces("application/json")]
    [Route("api/Message")]
    public class MessageController : Controller
    {



        private DataContext _context;
        public MessageController(DataContext context)
        {

            _context = context;
        }



        [HttpPost("PostMessage")]
        public async Task<IActionResult> PostMessage([FromBody] Message message)
        {
            await _context.Message.AddAsync(message);
            await _context.SaveChangesAsync();

            return Ok(message);
        }


        private List<int> receivedIdList;


        [HttpGet("GetMessage")]
        public async Task<IActionResult> GetMessage(int user_id)
        {
            var sender = await _context.Message.Where(m => m.sender_id.Equals(user_id)).OrderBy(t => t.receiver_id).ToListAsync();
            receivedIdList = new List<int>();
            sender.ForEach(m =>
            {
                if (!receivedIdList.Contains(m.receiver_id))
                {
                    receivedIdList.Add(m.receiver_id);

                }

            });

            var receiver = await _context.Message.Where(m => m.receiver_id.Equals(user_id)).OrderBy(t => t.sender_id).ToListAsync();

            receiver.ForEach(m =>
            {
                if (!receivedIdList.Contains(m.sender_id))
                {
                    receivedIdList.Add(m.sender_id);
                }

            });

            var returnMessage = new List<ReturnMessage>();

            receivedIdList.ForEach(m =>
            {

                var rm = new ReturnMessage() { user = GetProfile(m), messageList = GetMessageList(m, user_id) };


                var prListId = new List<int>();
                var prList = new List<ReturnProduct>();

                if (rm.messageList.Count > 0)
                {
                    rm.messageList.ForEach(t =>
                    {

                        if (!prListId.Contains(t.product_id))
                        {
                            prListId.Add(t.product_id);
                            var msgList = rm.messageList.Where(s => s.product_id.Equals(t.product_id)).ToList();
                            var _rm = new ReturnMessage
                            {
                                user = rm.user,
                                messageList = msgList,
                                product = GetProductById(t.product_id)
                            };
                            returnMessage.Add(_rm);
                        }

                    });

                }

                


            });

            return Ok(returnMessage);
        }

        private List<Message> GetMessageList(int receiver_id, int user_id)
        {
            var sender = _context.Message.Where(m => m.sender_id.Equals(user_id) && m.receiver_id.Equals(receiver_id)).ToList();
            var receiver = _context.Message.Where(m => m.receiver_id.Equals(user_id) && m.sender_id.Equals(receiver_id)).ToList();
            var messageList = new List<Message>();

            messageList.AddRange(sender);
            messageList.AddRange(receiver);

            return messageList.OrderBy(o => o.id).ToList();

        }

        private UserProfile GetProfile(int id)
        {

            var user = _context.Userr.Where(u => u.id.Equals(id)).FirstOrDefault();

            var image = _context.images.Where(i => i.uid.Equals(user.id) && i.description.Equals("profile")).FirstOrDefault();

            var city = _context.City.Where(c => c.id.Equals(user.city_id)).FirstOrDefault();



            var _user = new UserProfile()
            {
                id = user.id,
                city_id = user.city_id,
                surname = user.surname,
                name = user.name,
                email = user.email,
                photo_url = image.photo_url,
                city = city.city_name,
                iframe = city.iframe

            };


            return _user;
        }


        private ReturnProduct GetProductById(int id)
        {

            var product = _context.Product.Where(p => p.id.Equals(id)).FirstOrDefault();


            var category = _context.Category.Where(c => c.id.Equals(product.category_id)).FirstOrDefault();


            var image = _context.images.Where(i => i.product_id.Equals(product.id)).FirstOrDefault();



            var rProducts = new ReturnProduct()
            {
                id = product.id,
                header = product.header,
                description = product.description,
                price = product.price,
                image_list = new List<string> { image.photo_url },
                category = category == null ? "" : category.name,

            };


            return rProducts;
        }

    }
}
