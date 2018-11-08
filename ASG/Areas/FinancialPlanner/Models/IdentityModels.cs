//using System.Data.Entity;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace ASG.Areas.FinancialPlanner.Models
//{
//    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
//    public class ApplicationUser : IdentityUser
//    {

//        public string FirstName { get; set; }
//        public string LastName { get; set; }
//        public string DisplayName { get; set; }
//        [NotMapped]
//        public string FullName
//        {
//            get
//            {
//                return $"{ FirstName} {LastName}";
//            }
//        }


//        public ApplicationUser()
//        {

//            Transactions = new HashSet<Transaction>();
//            Accounts = new HashSet<FinancialAccount>();
//            Budgets = new HashSet<Budget>();


//        }
//        public virtual ICollection<Budget> Budgets { get; set; }
//        public virtual ICollection<FinancialAccount> Accounts { get; set; }
//        public virtual ICollection<Transaction> Transactions { get; set; }
//        public virtual ICollection<TransactionCategory> TransactionCategories { get; set; }
//        public virtual Household Household { get; set; }
//        public int? HouseholdId { get; set; }
//        public int? TransactionCategoryId { get; set; }


//        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
//        {
//            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
//            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
//            // Add custom user claims here
//            userIdentity.AddClaim(new Claim("HouseholdId", HouseholdId.ToString()));
//            return userIdentity;
//        }
//    }

//    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
//    {
//        public ApplicationDbContext()
//            : base("DefaultConnection", throwIfV1Schema: false)
//        {
//        }

//        public static ApplicationDbContext Create()
//        {
//            return new ApplicationDbContext();
//        }
//        public DbSet<FinancialAccount> Accounts { get; set; }
//        public DbSet<FinancialAccountType> AccountTypes { get; set; }
//        public DbSet<Budget> Budgets { get; set; }
//        public DbSet<BudgetItem> BudgetItems { get; set; }
//        public DbSet<Household> Households { get; set; }
//        public DbSet<Transaction> Transactions { get; set; }
//        public DbSet<TransactionCategory> TransactionCategories { get; set; }
//        public DbSet<TransactionType> TransactionTypes { get; set; }
//        public DbSet<BudgetDurationPeriod> BudgetDurationPeriods { get; set; }

//        public DbSet<Invitations> Invitations { get; set; }
//    }
//}