using NewJoineeBOT.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NewJoineeBOT.Utility
{
    public class UserRepository
    {
        FeedbackDbContext context;
        public FeedbackDbContext Context { get { return context; } }
        public UserRepository()
        {
            context = new FeedbackDbContext();
        }

        //public Feedback FetchEmployeeDetails(string rating)
        //{
        //    Feedback employee;
        //    try
        //    {
        //        employee = (from r in Context.Feedbacks
        //                    where r.Rating == rating
        //                    select r).FirstOrDefault();
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    return employee;
        //}

        public bool InsertFeedback(Feedback feedback)
        {
            bool status = false;
            try
            {
                Context.Feedbacks.Add(feedback);
                Context.SaveChanges(); 
                status = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return status;
        }

      
    }
}
