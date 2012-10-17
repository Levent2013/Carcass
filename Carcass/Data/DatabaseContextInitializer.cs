using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Data.Entity;

using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;

using WebMatrix.WebData;
using Carcass.Infrastructure;
using Carcass.Data.Entities;

namespace Carcass.Data
{
    public class DatabaseContextInitializer : DropCreateDatabaseIfModelChanges<DatabaseContext>
    {
        private static object _initMembershipLock = new object();

        public DatabaseContextInitializer()
        {
        }

        /// <summary>
        /// Fill the database with inital data (only at development stage)
        /// </summary>
        /// <param name="context">Current DB context</param>
        protected override void Seed(DatabaseContext context)
        {
            LoadExamplesData(context);

            context.SaveChanges();
            InitializeMembership(context); 
        }

        public static void InitializeMembership(DatabaseContext context)
        {
            lock (_initMembershipLock)
            {
                // Reset WebSecurity internal state - hack, but there is no other way
                ResetWebSecurityInitialization();

                WebSecurity.InitializeDatabaseConnection(
                    "DefaultConnection",
                    "Users",
                    "UserEntityId",
                    "UserName",
                    autoCreateTables: true);

                // add default roles
                if (!Roles.RoleExists(AppConstants.AdministratorsGroup))
                    Roles.CreateRole(AppConstants.AdministratorsGroup);
                if (!Roles.RoleExists(AppConstants.UsersGroup))
                    Roles.CreateRole(AppConstants.UsersGroup);
            }
        }

        /// <summary>
        /// Reset WebSecurity internal state if database 
        /// was updated but WebSecurity.Initialized still true.
        /// <remarks>
        /// Tested with WebMatrix 2.0.0.0
        /// </remarks>
        /// </summary>
        private static void ResetWebSecurityInitialization()
        {
            var initializedProperty = typeof(WebSecurity).GetProperty("Initialized",
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetProperty);
            if (initializedProperty !=null)
                initializedProperty.SetValue(null, false);

            var membership = Membership.Provider as SimpleMembershipProvider;
            if (membership != null)
            {
                initializedProperty = membership.GetType().GetProperty("InitializeCalled",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetProperty);
                if (initializedProperty != null)
                    initializedProperty.SetValue(membership, false);
            }

            var simpleRoles = Roles.Provider as SimpleRoleProvider;
            if (simpleRoles != null)
            {
                initializedProperty = simpleRoles.GetType().GetProperty("InitializeCalled",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetProperty);
                if (initializedProperty != null)
                    initializedProperty.SetValue(simpleRoles, false);
            }
        }

        private void LoadExamplesData(DatabaseContext context)
        {
            var random = new Random(DateTime.Now.Millisecond);

            var post = context.BlogPosts.Create();
            post.DateCreated = new DateTime(2012, 8, 20, random.Next(24), random.Next(60), 0);
            post.DateModified = post.DateCreated.AddDays(random.Next(50));
            post.Title = "Bootstrap 2.1.0 released";
            post.Origin = "http://blog.getbootstrap.com/2012/08/20/bootstrap-2-1-0-released/";
            post.Content = @"
 <p>After a smaller 2.0.4 release, we've got another huge update that resolves tons of bugs, improves the flexibility and durability of our code, and introduces a few awesome new features. It's a big release wrapped in a brand new set of docs and we couldn't be more stoked to launch it.</p>

<h2>tl;dr</h2>

<p>New docs, affix plugin, submenus on dropdowns, block buttons, image styles, fluid grid offsets, new navbar, increased font-size and line-height, 120+ closed bugs, and more. <a href=""http://getbootstrap.com"">Go get it.</a></p>

<h2>Improved documentation</h2>

<p>In addition to sporting a fresh visual style, the content of our docs has been overhauled once again. Good-bye long-winded marketing copy and multiple columns, hello succinct and directive single-column documentation. We've got a new tagline, new layout and navigation, and (more) clearer examples.</p>

<h2>Key changes and new features</h2>

<p>We had higher expectations for the number of new features in 2.1, but we toned it back to get this release out the door in a manageable form. We've still added some great new features and fixed tones of bugs, so here's a brief overview of what's new.</p>

<ul>
<li><strong>Submenu support on dropdowns.</strong> We avoided this for some time as we thought it would unnecessarily complicate things, but we've backtracked on that and opted to add them in. See the docs for more info.</li>
<li><strong>Affix JavaScript plugin.</strong> Make anything stick to the top of the viewport as you scroll with our newest plugin, built to power our new docs navigation.</li>
<li><strong>Block level buttons.</strong> Add <code>.btn-block</code> to a button to make it full-width.</li>
<li><strong>State classes on table rows.</strong> Better convey success, warning, and error messages in tables.</li>
<li><strong>Improved disabled states on navs and dropdowns.</strong> Where by ""improve"" we mean actually present&mdash;just add <code>.disabled</code> to the <code>li</code>.</li>
<li><strong>The navbar component is now white by default, with an optional class to darken it.</strong> In order to have two options for the navbar, light and dark, we needed to have better defaults. Having a lighter default navbar means fewer lines of code as we don't need to override anything for the basic navbar functionality and the override it again to restore the default styles. Missing the black? Just add <code>.navbar-inverse</code> to get the dark gray navbar back.</li>
<li><strong>Improved prepended and appended inputs.</strong> No need to place the <code>input</code> and <code>.add-on</code> on the same line of code (sorry about that by the way). Break them up as you like and they'll still stick together with some crafty <code>font-size</code> working.</li>
<li><strong>New base font-size and line-height.</strong> 13px/18px is out, 14px/20px is in. This also has changed the size, line-height, and margin of our heading elements, buttons, and more.</li>
<li><strong>Added variable for navbar collapse trigger point.</strong> Instead of a fixed pixel width of <code>980px</code> for triggering the collapsed navbar, we now use a variable so you can customize it for your projects.</li>
<li><strong>Fluid grid offsets.</strong> Thanks to our contributors, the fluid grid now has offset support to match our fixed (pixel) grid.</li>
<li><strong>Fluid grid system variables are no longer fixed percentages.</strong> Instead, they are calculated in LESS with <code>percentage()</code> based on the default, fixed-width grid variables. Math!</li>
<li><strong>Removed LESS docs page.</strong> We will no longer document variables and mixins in two places&mdash;rewriting that stuff was painful and took too much time. Instead, just checkout the .less files.</li>
</ul>
";
            context.BlogPosts.Add(post);

            post = context.BlogPosts.Create();
            post.DateCreated = new DateTime(2012, 4, 19, random.Next(24), random.Next(60), 0);
            post.DateModified = post.DateCreated.AddDays(random.Next(50));
            post.Title = "Bootstrap, JSHint, and Recess";
            post.Origin = "http://blog.getbootstrap.com/2012/04/19/bootstrap-jshint-and-recess/";
            post.Content = @"
<p>This last week we've added two new developer tools to the Bootstrap tool chain and I wanted to take a minute to let you know a little bit more about what they are, why we've added them, and why it matters.</p>

<h3><a href=""http://www.jshint.com"">JSHint</a></h3>

<p>JSHint is an awesome community-driven linting tool, used to detect errors and potential problems in your JS, and to enforce coding conventions. It's super flexible and can easily adapt to whichever coding guidelines and environment you expect your code to execute in.</p>
<p>As of 2.0.3, all JS (including tests) will be run through JSHint as a part of the build process.
We're hoping that this will both catch potentially unsafe syntax as well as encourage a convention around Bootstap's JavaScript style.</p>
<p>To begin with, Bootstrap's JS will use the following options (stored in a .jshintrc file in the js dir):</p>
<pre>
{
    ""validthis"" : true
  , ""laxcomma""  : true
  , ""laxbreak""  : true
  , ""browser""   : true
  , ""boss""      : true
  , ""expr""      : true
  , ""asi""       : true
}
</pre>


<p>We hope this will make it a little easier for those looking to contribute to Bootstrap, and lessen the pain around pull requests with divergent styles. If you haven't played with JSHint, you should definitely take a moment to <a href=""http://www.jshint.com/"">check it out right now</a>!</p>
<h3><a href=""http://twitter.github.com/recess"">Recess</a></h3>
<p>Recess is a project developed at Twitter to help support our internal style guide.</p>
<p><img src=""http://f.cl.ly/items/3R2v3e1G2P2S0y020j1D/Screen%20Shot%202012-04-19%20at%2012.57.15%20PM.png"" alt=""null"" /></p>
<p>At it’s core, Recess is a linter, but with the added ability to compile and/or reformat your css/less files: normalizing whitespace, stripping 0 values, reordering properties, and any other safe stylistic optimizations it can find.</p>
<p>What this means is that instead of just telling you where you have problems, you can actually tell Recess to just go ahead and tidy your code up for you.</p>
<p>To begin with, we're only using Recess in this manner — as a compiler for our Less (rather than the lessc compiler directly). This gives us strict control over the output of Bootstrap and let's Mark and I really geekout, which we're super excited about (we like things to be perfect... we're nerds like that).</p>
<h3><a href=""http://i.imgur.com/LnriU.gif"">The Future</a></h3>
<p>Eventually we'd like to try to roll these tools (along with our unit tests) into some sort of continuous integration service. At Twitter, we're already using travis-ci on a number of our other projects (Hogan.js, Recess), so we may follow suit with Bootstrap soon. This will make it even easier for us to accept pull requests from the community, as we'll be able to see all our tests passing! :)</p>
<p>Anyways — that's all for now. As always, if you have any questions or feedback, let us know! thanks!</p>
<p>&lt;3</p>
";
            context.BlogPosts.Add(post);
        }


    }
}