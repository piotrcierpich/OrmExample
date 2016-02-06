Deciding when to use Lazy Load is all about deciding how much you want to pull back from the database as you load an object, and how many database calls that will require. It’s usually pointless to use Lazy Load on a field that’s stored in the same row as the rest of the object, because most of the time it doesn’t cost any more to bring back extra data in a call, even if the data field is quite large—such as a Serialized LOB (272). That means it’s usually only worth considering Lazy Load if the field requires an extra database call to access.
In performance terms it’s about deciding when you want to take the hit of bringing back the data. Often it’s a good idea to bring everything you’ll need in one call so you have it in place, particularly if it corresponds to a single interaction with a UI. The best time to use Lazy Load is when it involves an extra call and the data you’re calling isn’t used when the main object is used.
Adding Lazy Load does add a little complexity to the program, so my preference is not to use it unless I actively think I’ll need it.
Example: Lazy Initialization (Java)
The essence of lazy initialization is code like this:

class Supplier...

   public List getProducts() {
      if (products == null) products = Product.findForSupplier(getID());
      return products;
   }

In this way the first access of the products field causes the data to be loaded from the database.
Example: Virtual Proxy (Java)
The key to the virtual proxy is providing a class that looks like the actual class you normally use but that actually holds a simple wrapper around the real class. Thus, a list of products for a supplier would be held with a regular list field.
Click here to view code image

class SupplierVL...

   private List products;

The most complicated thing about producing a list proxy like this is setting it up so that you can provide an underlying list that’s created only when it’s accessed. To do this we have to pass the code that’s needed to create the list into the virtual list when it’s instantiated. The best way to do this in Java is to define an interface for the loading behavior.
Click here to view code image

public interface VirtualListLoader  {
   List load();
}

Then we can instantiate the virtual list with a loader that calls the appropriate mapper method.
Click here to view code image

class SupplierMapper...

   public static class ProductLoader implements VirtualListLoader  {
      private Long id;
      public ProductLoader(Long id) {
         this.id= id;
      }
      public List load() {
         return ProductMapper.create().findForSupplier(id);
      }
   }

During the load method we assign the product loader to the list field.
Click here to view code image

class SupplierMapper...

   protected DomainObject doLoad(Long id, ResultSet rs) throws SQLException {
      String nameArg = rs.getString(2);
      SupplierVL result = new SupplierVL(id, nameArg);
      result.setProducts(new VirtualList(new ProductLoader(id)));
      return result;
   }

The virtual list’s source list is self-encapsulated and evaluates the loader on first reference.
Click here to view code image

class VirtualList...

   private List source;
   private VirtualListLoader loader;
   public VirtualList(VirtualListLoader loader) {
      this.loader = loader;
   }
   private List getSource() {
      if (source == null) source = loader.load();
      return source;
   }

The regular list methods to delegate are then implemented to the source list.
Click here to view code image

class VirtualList...

   public int size() {
      return getSource().size();
   }
   public boolean isEmpty() {
      return getSource().isEmpty();
   }
   // ... and so on for rest of list methods

This way the domain class knows nothing about how the mapper class does the Lazy Load. Indeed, the domain class isn’t even aware that there is a Lazy Load.
Example: Using a Value Holder (Java)
A value holder can be used as a generic Lazy Load. In this case the domain type is aware that something is afoot, since the product field is typed as a value holder. This fact can be hidden from clients of the supplier by the getting method.
Click here to view code image

class SupplierVH...

   private ValueHolder products;
   public List getProducts() {
      return (List) products.getValue();
   }

The value holder itself does the Lazy Load behavior. It needs to be passed the necessary code to load its value when it’s accessed. We can do this by defining a loader interface.
Click here to view code image

class ValueHolder...

   private Object value;
   private ValueLoader loader;
   public ValueHolder(ValueLoader loader) {
      this.loader = loader;
   }
   public Object getValue() {
      if (value == null) value = loader.load();
      return value;
   }
public interface ValueLoader  {
   Object load();
}

A mapper can set up the value holder by creating an implementation of the loader and putting it into the supplier object.
Click here to view code image

class SupplierMapper...

   protected DomainObject doLoad(Long id, ResultSet rs) throws SQLException {
      String nameArg = rs.getString(2);
      SupplierVH result = new SupplierVH(id, nameArg);
      result.setProducts(new ValueHolder(new ProductLoader(id)));
      return result;
   }
   public static class ProductLoader implements ValueLoader  {
      private Long id;
      public ProductLoader(Long id) {
         this.id= id;
      }
      public Object load() {
         return ProductMapper.create().findForSupplier(id);
      }
   }

Example: Using Ghosts (C#)
Much of the logic for making objects ghosts can be built into Layer Supertypes (475). As a consequence, if you use ghosts you tend to see them used everywhere. I’ll begin our exploration of ghosts by looking at the domain object Layer Supertype (475). Each domain object knows if it’s a ghost or not.
Click here to view code image

class Domain Object...

      LoadStatus Status;
      public DomainObject (long key) {
         this.Key = key;
      }
      public Boolean IsGhost {
         get {return Status == LoadStatus.GHOST;}
      }
      public Boolean IsLoaded  {
         get {return Status == LoadStatus.LOADED;}
      }
      public void MarkLoading() {
         Debug.Assert(IsGhost);
         Status = LoadStatus.LOADING;
      }
      public void MarkLoaded() {
         Debug.Assert(Status == LoadStatus.LOADING);
         Status = LoadStatus.LOADED;
      }
   enum LoadStatus  {GHOST, LOADING, LOADED};

Domain objects can be in three states: ghost, loading, and loaded. I like to wrap status information with read-only properties and explicit status change methods.
The most intrusive element of ghosts is that every accessor needs to be modified so that it will trigger a load if the object actually is a ghost.
Click here to view code image

class Employee...

      public String Name {
         get {
            Load();
            return  _name;
         }
         set {
            Load();
            _name = value;
         }
      }
      String  _name;

class Domain Object...

      protected void Load() {
         if (IsGhost)
            DataSource.Load(this);
      }

Such a need, which is annoying to remember, is an ideal target for aspect-oriented programming for post-processing the bytecode.
In order for the loading to work, the domain object needs to call the correct mapper. However, my visibility rules dictate that the domain code may not see the mapper code. To avoid the dependency, I need to use an interesting combination of Registry (480) and Separated Interface (476) (Figure 11.4). I define a Registry (480) for the domain for data source operations.
Click here to view code image

class DataSource...

      public static void Load (DomainObject obj) {
         instance.Load(obj);
      }

Image
Figure 11.4. Classes involved in loading a ghost.

The instance of the data source is defined using an interface.
Click here to view code image

class DataSource...

   public interface IDataSource {
      void Load (DomainObject obj);
   }

A registry of mappers, defined in the data source layer, implements the data source interface. In this case I’ve put the mappers in a dictionary indexed by domain type. The load method finds the correct mapper and tells it to load the appropriate domain object.
Click here to view code image

class MapperRegistry : IDataSource...

      public void Load (DomainObject obj) {
         Mapper(obj.GetType()).Load (obj);
      }
      public static Mapper Mapper(Type type) {
         return (Mapper) instance.mappers[type];
      }
      IDictionary mappers = new Hashtable();

The preceding code shows how the domain objects interact with the data source. The data source logic uses Data Mappers (165). The update logic on the mappers is the same as in the case with no ghosts—the interesting behavior for this example lies in the finding and loading behavior.
Concrete mapper classes have their own find methods that use an abstract method and downcast the result.
Click here to view code image

class EmployeeMapper...

      public Employee Find (long key) {
         return (Employee) AbstractFind(key);
      }

class Mapper...

      public DomainObject AbstractFind (long key) {
         DomainObject result;
         result = (DomainObject) loadedMap[key];
         if (result == null) {
            result = CreateGhost(key);
            loadedMap.Add(key, result);
         }
         return result;
      }
      IDictionary loadedMap = new Hashtable();
      public abstract DomainObject CreateGhost(long key);

class EmployeeMapper...

      public override DomainObject CreateGhost(long key) {
         return new Employee(key);
      }

As you can see, the find method returns an object in its ghost state. The actual data does not come from the database until the load is triggered by accessing a property on the domain object.
Click here to view code image

class Mapper...

      public void Load (DomainObject obj) {
         if (! obj.IsGhost) return;
         IDbCommand comm = new OleDbCommand(findStatement(), DB.connection);
         comm.Parameters.Add(new OleDbParameter("key",obj.Key));
         IDataReader reader = comm.ExecuteReader();
         reader.Read();
         LoadLine (reader, obj);
         reader.Close();
      }
      protected abstract String findStatement();
      public void LoadLine (IDataReader reader, DomainObject obj) {
         if (obj.IsGhost) {
            obj.MarkLoading();
            doLoadLine (reader, obj);
            obj.MarkLoaded();
         }
      }
      protected abstract void doLoadLine (IDataReader reader, DomainObject obj);

As is common with these examples, the Layer Supertype (475) handles all of the abstract behavior and then calls an abstract method for a particular subclass to play its part. For this example I’ve used a data reader, a cursor-based approach that’s the more common for the various platforms at the moment. I’ll leave it to you to extend this to a data set, which would actually be more suitable for most cases in .NET.
For this employee object, I’ll show three kinds of property: a name that’s a simple value, a department that’s a reference to another object, and a list of timesheet records that shows the case of a collection. All are loaded together in the subclass’s implementation of the hook method.
Click here to view code image

class EmployeeMapper...

      protected override void doLoadLine (IDataReader reader, DomainObject obj) {
         Employee employee = (Employee) obj;
         employee.Name = (String) reader["name"];
         DepartmentMapper depMapper =
            (DepartmentMapper) MapperRegistry.Mapper(typeof(Department));
         employee.Department = depMapper.Find((int) reader["departmentID"]);
         loadTimeRecords(employee);
      }

The name’s value is loaded simply by reading the appropriate column from the data reader’s current cursor. The department is read by using the find method on the department’s mapper object. This will end up setting the property to a ghost of the department; the department’s data will only be read when the department object itself is accessed.
The collection is the most complicated case. To avoid ripple loading, it’s important to load all the time records in a single query. For this we need a special list implementation that acts as a ghost list. This list is just a thin wrapper around a real list object, to which all the real behavior is just delegated. The only thing the ghost does is ensure that any accesses to the real list triggers a load.
Click here to view code image

class DomainList...

      IList data  {
         get {
            Load();
            return  _data;
         }
         set {_data = value;}
      }
      IList  _data = new ArrayList();
      public int Count {
         get {return data.Count;}
      }

The domain list class is used by domain objects and is part of the domain layer. The actual loading needs access to SQL commands, so I use a delegate to define a loading function than can be supplied by the mapping layer.
Click here to view code image

class DomainList...

      public void Load () {
         if (IsGhost) {
            MarkLoading();
            RunLoader(this);
            MarkLoaded();
         }
      }
      public delegate void Loader(DomainList list);
      public Loader RunLoader;

Think of a delegate as a special variety of Separated Interface (476) for a single function. Indeed, declaring an interface with a single function in it is a reasonable alternative way of doing this.
Image
Figure 11.5. The load sequence for a ghost.

Image
Figure 11.6. Classes for a ghost list. As yet there’s no accepted standard for showing delegates in UML models. This is my current approach.

The loader itself has properties to specify the SQL for the load and mapper to use for mapping the time records. The employee’s mapper sets up the loader when it loads the employee object.
Click here to view code image

class EmployeeMapper...

      void loadTimeRecords(Employee employee) {
         ListLoader loader = new ListLoader();
         loader.Sql = TimeRecordMapper.FIND_FOR_EMPLOYEE_SQL;
         loader.SqlParams.Add(employee.Key);
         loader.Mapper = MapperRegistry.Mapper(typeof(TimeRecord));
         loader.Attach((DomainList) employee.TimeRecords);
      }
class ListLoader...

      public String Sql;
      public IList SqlParams = new ArrayList();
      public Mapper Mapper;

Since the syntax for the delegate assignment is a bit complicated, I’ve given the loader an attach method.
Click here to view code image

class ListLoader...

      public void Attach (DomainList list) {
         list.RunLoader = new DomainList.Loader(Load);
      }

When the employee is loaded, the time records collection stays in a ghost state until one of the access methods fires to trigger the loader. At this point the loader executes the query to fill the list.
Click here to view code image

class ListLoader...

      public void Load (DomainList list) {
         list.IsLoaded = true;
         IDbCommand comm = new OleDbCommand(Sql, DB.connection);
         foreach (Object param in SqlParams)
            comm.Parameters.Add(new OleDbParameter(param.ToString(),param));
         IDataReader reader = comm.ExecuteReader();
         while (reader.Read()) {
            DomainObject obj = GhostForLine(reader);
            Mapper.LoadLine(reader, obj);
            list.Add (obj);
         }
         reader.Close();
      }
      private DomainObject GhostForLine(IDataReader reader) {
         return Mapper.AbstractFind((System.Int32)reader[Mapper.KeyColumnName]);
      }