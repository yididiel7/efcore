﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using ISingletonInterceptor = Microsoft.EntityFrameworkCore.Diagnostics.ISingletonInterceptor;

namespace Microsoft.EntityFrameworkCore;

public abstract class MaterializationInterceptionTestBase : SingletonInterceptorsTestBase
{
    protected MaterializationInterceptionTestBase(SingletonInterceptorsFixtureBase fixture)
        : base(fixture)
    {
    }

    [ConditionalTheory]
    [InlineData(false)]
    [InlineData(true)]
    public virtual void Binding_interceptors_are_used_by_queries(bool inject)
    {
        var interceptors = new[]
        {
            new TestBindingInterceptor("1"),
            new TestBindingInterceptor("2"),
            new TestBindingInterceptor("3"),
            new TestBindingInterceptor("4")
        };

        using var context = CreateContext(interceptors, inject);

        context.AddRange(
            new Book { Title = "Amiga ROM Kernel Reference Manual" },
            new Book { Title = "Amiga Hardware Reference Manual" });

        context.SaveChanges();
        context.ChangeTracker.Clear();

        var results = context.Set<Book>().ToList();
        Assert.All(results, e => Assert.Equal("4", e.MaterializedBy));
        Assert.All(interceptors, i => Assert.Equal(1, i.CalledCount));
    }

    [ConditionalTheory]
    [InlineData(false)]
    [InlineData(true)]
    public virtual void Binding_interceptors_are_used_when_creating_instances(bool inject)
    {
        var interceptors = new[]
        {
            new TestBindingInterceptor("1"),
            new TestBindingInterceptor("2"),
            new TestBindingInterceptor("3"),
            new TestBindingInterceptor("4")
        };

        using var context = CreateContext(interceptors, inject);

        var materializer = context.GetService<IEntityMaterializerSource>();
        var book = (Book)materializer.GetEmptyMaterializer(context.Model.FindEntityType(typeof(Book))!)(
            new MaterializationContext(ValueBuffer.Empty, context));

        Assert.Equal("4", book.MaterializedBy);
        Assert.All(interceptors, i => Assert.Equal(1, i.CalledCount));
    }

    [ConditionalTheory]
    [InlineData(false)]
    [InlineData(true)]
    public virtual void Intercept_query_materialization_for_empty_constructor(bool inject)
    {
        var creatingInstanceCount = 0;
        var createdInstanceCount = 0;
        var initializingInstanceCount = 0;
        var initializedInstanceCount = 0;
        LibraryContext? context = null;
        var ids = new HashSet<Guid>();
        var titles = new HashSet<string?>();
        var authors = new HashSet<string?>();

        var interceptors = new[]
        {
            new ValidatingMaterializationInterceptor(
                (data, instance, method) =>
                {
                    Assert.Same(context, data.Context);
                    Assert.Same(data.Context.Model.FindEntityType(typeof(Book)), data.EntityType);

                    var idProperty = data.EntityType.FindProperty(nameof(Book.Id))!;
                    var id = data.GetPropertyValue<Guid>(nameof(Book.Id))!;
                    Assert.Equal(id, data.GetPropertyValue(nameof(Book.Id)));
                    Assert.Equal(id, data.GetPropertyValue<Guid>(idProperty));
                    Assert.Equal(id, data.GetPropertyValue(idProperty));
                    ids.Add(id);

                    var titleProperty = data.EntityType.FindProperty(nameof(Book.Title))!;
                    var title = data.GetPropertyValue<string?>(nameof(Book.Title));
                    Assert.Equal(title, data.GetPropertyValue(nameof(Book.Title)));
                    Assert.Equal(title, data.GetPropertyValue<string?>(titleProperty));
                    Assert.Equal(title, data.GetPropertyValue(titleProperty));
                    titles.Add(title);

                    var authorProperty = data.EntityType.FindProperty("Author")!;
                    var author = data.GetPropertyValue<string?>("Author");
                    Assert.Equal(author, data.GetPropertyValue("Author"));
                    Assert.Equal(author, data.GetPropertyValue<string?>(authorProperty));
                    Assert.Equal(author, data.GetPropertyValue(authorProperty));
                    authors.Add(author);

                    switch (method)
                    {
                        case nameof(IMaterializationInterceptor.CreatingInstance):
                            creatingInstanceCount++;
                            Assert.Null(instance);
                            break;
                        case nameof(IMaterializationInterceptor.CreatedInstance):
                            createdInstanceCount++;
                            Assert.IsType<Book>(instance);
                            Assert.Equal(Guid.Empty, ((Book)instance!).Id);
                            Assert.Null(((Book)instance!).Title);
                            break;
                        case nameof(IMaterializationInterceptor.InitializingInstance):
                            initializingInstanceCount++;
                            Assert.IsType<Book>(instance);
                            Assert.Equal(Guid.Empty, ((Book)instance!).Id);
                            Assert.Null(((Book)instance!).Title);
                            break;
                        case nameof(IMaterializationInterceptor.InitializedInstance):
                            initializedInstanceCount++;
                            Assert.IsType<Book>(instance);
                            Assert.Equal(id, ((Book)instance!).Id);
                            Assert.Equal(title, ((Book)instance!).Title);
                            break;
                    }
                })
        };

        using (context = CreateContext(interceptors, inject))
        {
            var books = new[]
            {
                new Book { Title = "Amiga ROM Kernel Reference Manual" }, new Book { Title = "Amiga Hardware Reference Manual" }
            };

            context.AddRange(books);

            context.Entry(books[0]).Property("Author").CurrentValue = "Commodore Business Machines Inc.";
            context.Entry(books[1]).Property("Author").CurrentValue = "Agnes";

            context.SaveChanges();
            context.ChangeTracker.Clear();

            var results = context.Set<Book>().Where(e => books.Select(e => e.Id).Contains(e.Id)).ToList();
            Assert.Equal(2, results.Count);

            Assert.Equal(2, creatingInstanceCount);
            Assert.Equal(2, createdInstanceCount);
            Assert.Equal(2, initializingInstanceCount);
            Assert.Equal(2, initializedInstanceCount);

            Assert.Equal(2, ids.Count);
            Assert.Equal(2, titles.Count);
            Assert.Equal(2, authors.Count);
            Assert.Contains(ids, t => t == books[0].Id);
            Assert.Contains(ids, t => t == books[1].Id);
            Assert.Contains(titles, t => t == "Amiga ROM Kernel Reference Manual");
            Assert.Contains(titles, t => t == "Amiga Hardware Reference Manual");
            Assert.Contains(authors, t => t == "Commodore Business Machines Inc.");
            Assert.Contains(authors, t => t == "Agnes");
        }
    }

    [ConditionalTheory]
    [InlineData(false)]
    [InlineData(true)]
    public virtual void Intercept_query_materialization_for_full_constructor(bool inject)
    {
        var creatingInstanceCount = 0;
        var createdInstanceCount = 0;
        var initializingInstanceCount = 0;
        var initializedInstanceCount = 0;
        LibraryContext? context = null;
        var ids = new HashSet<Guid>();
        var titles = new HashSet<string?>();
        var authors = new HashSet<string?>();

        var interceptors = new[]
        {
            new ValidatingMaterializationInterceptor(
                (data, instance, method) =>
                {
                    Assert.Same(context, data.Context);
                    Assert.Same(data.Context.Model.FindEntityType(typeof(Pamphlet)), data.EntityType);

                    var idProperty = data.EntityType.FindProperty(nameof(Pamphlet.Id))!;
                    var id = data.GetPropertyValue<Guid>(nameof(Pamphlet.Id))!;
                    Assert.Equal(id, data.GetPropertyValue(nameof(Pamphlet.Id)));
                    Assert.Equal(id, data.GetPropertyValue<Guid>(idProperty));
                    Assert.Equal(id, data.GetPropertyValue(idProperty));
                    ids.Add(id);

                    var titleProperty = data.EntityType.FindProperty(nameof(Pamphlet.Title))!;
                    var title = data.GetPropertyValue<string?>(nameof(Pamphlet.Title));
                    Assert.Equal(title, data.GetPropertyValue(nameof(Pamphlet.Title)));
                    Assert.Equal(title, data.GetPropertyValue<string?>(titleProperty));
                    Assert.Equal(title, data.GetPropertyValue(titleProperty));
                    titles.Add(title);

                    var authorProperty = data.EntityType.FindProperty("Author")!;
                    var author = data.GetPropertyValue<string?>("Author");
                    Assert.Equal(author, data.GetPropertyValue("Author"));
                    Assert.Equal(author, data.GetPropertyValue<string?>(authorProperty));
                    Assert.Equal(author, data.GetPropertyValue(authorProperty));
                    authors.Add(author);

                    switch (method)
                    {
                        case nameof(IMaterializationInterceptor.CreatingInstance):
                            creatingInstanceCount++;
                            Assert.Null(instance);
                            break;
                        case nameof(IMaterializationInterceptor.CreatedInstance):
                            createdInstanceCount++;
                            Assert.IsType<Pamphlet>(instance);
                            Assert.Equal(id, ((Pamphlet)instance!).Id);
                            Assert.Equal(title, ((Pamphlet)instance!).Title);
                            break;
                        case nameof(IMaterializationInterceptor.InitializingInstance):
                            initializingInstanceCount++;
                            Assert.IsType<Pamphlet>(instance);
                            Assert.Equal(id, ((Pamphlet)instance!).Id);
                            Assert.Equal(title, ((Pamphlet)instance!).Title);
                            break;
                        case nameof(IMaterializationInterceptor.InitializedInstance):
                            initializedInstanceCount++;
                            Assert.IsType<Pamphlet>(instance);
                            Assert.Equal(id, ((Pamphlet)instance!).Id);
                            Assert.Equal(title, ((Pamphlet)instance!).Title);
                            break;
                    }
                })
        };

        using (context = CreateContext(interceptors, inject))
        {
            var pamphlets = new[] { new Pamphlet(Guid.Empty, "Rights of Man"), new Pamphlet(Guid.Empty, "Pamphlet des pamphlets") };

            context.AddRange(pamphlets);

            context.Entry(pamphlets[0]).Property("Author").CurrentValue = "Thomas Paine";
            context.Entry(pamphlets[1]).Property("Author").CurrentValue = "Paul-Louis Courier";

            context.SaveChanges();
            context.ChangeTracker.Clear();

            var results = context.Set<Pamphlet>().Where(e => pamphlets.Select(e => e.Id).Contains(e.Id)).ToList();
            Assert.Equal(2, results.Count);

            Assert.Equal(2, creatingInstanceCount);
            Assert.Equal(2, createdInstanceCount);
            Assert.Equal(2, initializingInstanceCount);
            Assert.Equal(2, initializedInstanceCount);

            Assert.Equal(2, ids.Count);
            Assert.Equal(2, titles.Count);
            Assert.Equal(2, authors.Count);
            Assert.Contains(ids, t => t == pamphlets[0].Id);
            Assert.Contains(ids, t => t == pamphlets[1].Id);
            Assert.Contains(titles, t => t == "Rights of Man");
            Assert.Contains(titles, t => t == "Pamphlet des pamphlets");
            Assert.Contains(authors, t => t == "Thomas Paine");
            Assert.Contains(authors, t => t == "Paul-Louis Courier");
        }
    }

    [ConditionalTheory]
    [InlineData(false)]
    [InlineData(true)]
    public virtual void Multiple_materialization_interceptors_can_be_used(bool inject)
    {
        var interceptors = new ISingletonInterceptor[]
        {
            new CountingMaterializationInterceptor("A"),
            new TestBindingInterceptor("1"),
            new CountingMaterializationInterceptor("B"),
            new TestBindingInterceptor("2"),
            new TestBindingInterceptor("3"),
            new TestBindingInterceptor("4"),
            new CountingMaterializationInterceptor("C")
        };

        using var context = CreateContext(interceptors, inject);

        context.AddRange(
            new Book { Title = "Amiga ROM Kernel Reference Manual" },
            new Book { Title = "Amiga Hardware Reference Manual" });

        context.SaveChanges();
        context.ChangeTracker.Clear();

        var results = context.Set<Book>().ToList();
        Assert.All(results, e => Assert.Equal("4", e.MaterializedBy));
        Assert.All(interceptors.OfType<TestBindingInterceptor>(), i => Assert.Equal(1, i.CalledCount));

        Assert.All(results, e => Assert.Equal("ABC", e.CreatedBy));
        Assert.All(results, e => Assert.Equal("ABC", e.InitializingBy));
        Assert.All(results, e => Assert.Equal("ABC", e.InitializedBy));
    }

    protected class TestBindingInterceptor : IInstantiationBindingInterceptor
    {
        private readonly string _id;

        public TestBindingInterceptor(string id)
        {
            _id = id;
        }

        public int CalledCount { get; private set; }

        protected Book BookFactory()
            => new() { MaterializedBy = _id };

        public InstantiationBinding ModifyBinding(IEntityType entityType, string entityInstanceName, InstantiationBinding binding)
        {
            CalledCount++;

            return new FactoryMethodBinding(
                this,
                typeof(TestBindingInterceptor).GetTypeInfo().GetDeclaredMethod(nameof(BookFactory))!,
                new List<ParameterBinding>(),
                entityType.ClrType);
        }
    }

    protected class ValidatingMaterializationInterceptor : IMaterializationInterceptor
    {
        private readonly Action<MaterializationInterceptionData, object?, string> _validate;

        public ValidatingMaterializationInterceptor(
            Action<MaterializationInterceptionData, object?, string> validate)
        {
            _validate = validate;
        }

        public InterceptionResult<object> CreatingInstance(
            MaterializationInterceptionData materializationData, InterceptionResult<object> result)
        {
            _validate(materializationData, null, nameof(CreatingInstance));

            return result;
        }

        public object CreatedInstance(
            MaterializationInterceptionData materializationData, object instance)
        {
            _validate(materializationData, instance, nameof(CreatedInstance));

            return instance;
        }

        public InterceptionResult InitializingInstance(
            MaterializationInterceptionData materializationData, object instance, InterceptionResult result)
        {
            _validate(materializationData, instance, nameof(InitializingInstance));

            return result;
        }

        public object InitializedInstance(
            MaterializationInterceptionData materializationData, object instance)
        {
            _validate(materializationData, instance, nameof(InitializedInstance));

            return instance;
        }
    }

    protected class CountingMaterializationInterceptor : IMaterializationInterceptor
    {
        private readonly string _id;

        public CountingMaterializationInterceptor(string id)
        {
            _id = id;
        }

        public InterceptionResult<object> CreatingInstance(
            MaterializationInterceptionData materializationData, InterceptionResult<object> result)
            => result;

        public object CreatedInstance(
            MaterializationInterceptionData materializationData, object instance)
        {
            ((Book)instance).CreatedBy += _id;
            return instance;
        }

        public InterceptionResult InitializingInstance(
            MaterializationInterceptionData materializationData, object instance, InterceptionResult result)
        {
            ((Book)instance).InitializingBy += _id;
            return result;
        }

        public object InitializedInstance(
            MaterializationInterceptionData materializationData, object instance)
        {
            ((Book)instance).InitializedBy += _id;
            return instance;
        }
    }
}
