using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Autofac;
using Autofac.Integration.Mvc;

using Carcass.Common.Data;
using Carcass.Controllers;
using Carcass.Data.Entities;
using Subtext.TestLibrary;

namespace Carcass.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTest
    {
        private List<UserEntity> _users;

        static AccountControllerTest()
        {
            Common.Initialization.Init();
        }
        
        [TestInitialize]
        public void Init()
        {
            _users = new List<UserEntity>();
            for (var ndx = 0; ndx < 10; ++ndx) 
            {
                var postfix = ndx.ToString();
                var user = new UserEntity
                {
                    UserEntityId = ndx + 1,
                    DateRegistered = DateTime.Now,
                    UserName = "User" + postfix,
                    Email = String.Format("user.{0}@test.com", ndx)
                };

                _users.Add(user);
            }

            var builder = new ContainerBuilder();
            
            builder.RegisterType<QueryBuilder>().As<IQueryBuilder>();
            var usersRepo = new Repository<UserEntity>(_users);
            builder.RegisterInstance(usersRepo).As<IRepository<UserEntity>>().SingleInstance();
            
            var container = builder.Build();
            var resolver = new AutofacDependencyResolver(container);
            DependencyResolver.SetResolver(resolver); 
        }


        [TestMethod]
        public void Users()
        {
            using (var httpSimulator = new HttpSimulator())
            {
                httpSimulator.SimulateRequest();
                AccountController controller = new AccountController(DependencyResolver.Current.GetService<IQueryBuilder>());
                ViewResult result = controller.Users() as ViewResult;

                Assert.IsInstanceOfType(result.Model, typeof(IEnumerable<UserEntity>));
                var users = result.Model as IEnumerable<UserEntity>;

                Assert.AreEqual(users.Count(), _users.Count());
                Assert.AreEqual(users.First().UserEntityId, 1);
                Assert.AreEqual(users.Last().UserEntityId, 10);
            }
        }
    }
}
