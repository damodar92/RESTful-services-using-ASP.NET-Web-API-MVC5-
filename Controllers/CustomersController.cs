using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vidly.Models;
using System.Data.Entity;
using Vidly.ViewModels;

namespace Vidly.Controllers
{
    public class CustomersController : Controller
    {
        // GET: Customers
        private ApplicationDbContext _context;

        public CustomersController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        public ActionResult New()
        {
            var membershipTypes = _context.MembershipTypes.ToList();
            var viewModel = new CustomerFormViewModel
            {
                Customer = new Customer(),
                MembershipTypes = membershipTypes
            };
            return View("CustomerForm",viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Customer customer)
        {
            //To add the customer to the database, we have to use dbContext, which sets up, the connection with the database.
            //Once the Add method is called, we have to call the method that saves all the changes to the database.

            //Adding validation to the fields. We use ModelState property.

            if (!ModelState.IsValid)
            {
                var viewModel = new CustomerFormViewModel
                {
                    Customer = customer,
                    MembershipTypes = _context.MembershipTypes.ToList()
                };

                 return View("CustomerForm", viewModel);
            }
        
            if(customer.Id == 0)
            {
                _context.Customers.Add(customer);
            }
           else
            {
                var customerInDb = _context.Customers.Single(c => c.Id == customer.Id);
                customerInDb.Name = customer.Name;
                customerInDb.Birthdate = customer.Birthdate;
                customerInDb.MembershipTypeId = customer.MembershipTypeId;
                customerInDb.IsSubscribedToNewsletter = customer.IsSubscribedToNewsletter;
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Customers");
        }

        public ActionResult Index()
        {
            //Here in the below code, we have used Include(). This is because, in entity framework, the 
            //when displaying two seperate properties of a class, it is not possible to fetch individual properties seperately.
            //Therefore we have to include whichever proerty that needs to be displayed in the fect statement ie.e the statement that loads data.

            //Also, ToList() method is used. This is because, when the page is loaded, all the rows of data that are 
            //present in the database will be iterated only when an iteration function like IEnumerable is run on the data. 
            //To fetch data instantly, we use ToList() method. This executes on the database on page loaD, THUS AVOIDING THE LOAD DURING VIEW EXECUTION.
            // var customers = _context.Customers.Include(c => c.MembershipType).ToList();

            // return View(customers);
            return View();

        }

        public ActionResult Details(int id)
        {
            var selectedCustomer = _context.Customers.Include(c => c.MembershipType).SingleOrDefault(x => x.Id == id);

            if (selectedCustomer == null)
            {
                return HttpNotFound();
            }

            return View(selectedCustomer);
        }

        public ActionResult Edit(int id)
        {
            var customer = _context.Customers.SingleOrDefault(c => c.Id == id);

            if(customer == null)
            {
                return HttpNotFound();
            }

            var viewModel = new CustomerFormViewModel
            {
                Customer = customer,
                MembershipTypes = _context.MembershipTypes.ToList()

            };
            return View("CustomerForm", viewModel );
        }

        /* private IEnumerable<Customer> PopulateCustomers()
         {
             return new List<Customer>{
                 new Customer { Id= 1, Name="John Smith" },
                 new Customer { Id=2, Name="Mary Williams" }
             };
         }*/
    }
}