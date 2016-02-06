Unit of work umozliwia SaveChanges - opozniony zapis zmian wprowadzanych do bazy w ramach jednej transakcji
Dorobic generyczna klase mapujaca



   private List newObjects = new ArrayList();
   private List dirtyObjects = new ArrayList();
   private List removedObjects = new ArrayList();

The registration methods maintain the state of these lists. They must perform basic assertions such as checking that an ID isn’t null or that a dirty object isn’t being registered as new.
Click here to view code image

class UnitOfWork...

   public void registerNew(DomainObject obj) {
      Assert.notNull("id not null", obj.getId());
      Assert.isTrue("object not dirty", !dirtyObjects.contains(obj));
      Assert.isTrue("object not removed", !removedObjects.contains(obj));
      Assert.isTrue("object not already registered new", !newObjects.contains(obj));
      newObjects.add(obj);
   }
   public void registerDirty(DomainObject obj) {
      Assert.notNull("id not null", obj.getId());
      Assert.isTrue("object not removed", !removedObjects.contains(obj));
      if (!dirtyObjects.contains(obj) && !newObjects.contains(obj)) {
         dirtyObjects.add(obj);
      }
   }
   public void registerRemoved(DomainObject obj) {
      Assert.notNull("id not null", obj.getId());
      if (newObjects.remove(obj)) return;
      dirtyObjects.remove(obj);
      if (!removedObjects.contains(obj)) {
         removedObjects.add(obj);
      }
   }
   public void registerClean(DomainObject obj) {
      Assert.notNull("id not null", obj.getId());
   }
   
   
   class UnitOfWork...

   public void commit() {
      insertNew();
      updateDirty();
      deleteRemoved();
   }
   private void insertNew() {
      for (Iterator objects = newObjects.iterator();  objects.hasNext();) {
         MapperRegistry.getMapper(obj.getClass()).insert(obj);
         DomainObject obj = (DomainObject) objects.next();
      }
   }
   
   class UnitOfWork...

   private static ThreadLocal current = new ThreadLocal();
   public static void newCurrent() {
      setCurrent(new UnitOfWork());
   }
   public static void setCurrent(UnitOfWork uow) {
      current.set(uow);	
   }
   public static UnitOfWork getCurrent() {
      return (UnitOfWork) current.get();
   }

Now we can give our abstract domain object the marking methods to register itself with the current Unit of Work.
Click here to view code image

class DomainObject...

   protected void markNew() {
      UnitOfWork.getCurrent().registerNew(this);
   }
   protected void markClean() {
      UnitOfWork.getCurrent().registerClean(this);
   }
   protected void markDirty() {
      UnitOfWork.getCurrent().registerDirty(this);
   }
   protected void markRemoved() {
      UnitOfWork.getCurrent().registerRemoved(this);
   }

Concrete domain objects need to remember to mark themselves new and dirty where appropriate.
Click here to view code image

class Album...

   public static Album create(String name) {
      Album obj = new Album(IdGenerator.nextId(), name);
      obj.markNew();
      return obj;
   }
   public void setTitle(String title) {
      this.title = title;
      markDirty();
   }
   
   
   class EditAlbumScript...

   public static void updateTitle(Long albumId, String title) {
      UnitOfWork.newCurrent();
      Mapper mapper = MapperRegistry.getMapper(Album.class);
      Album album = (Album) mapper.find(albumId);
      album.setTitle(title);
      UnitOfWork.getCurrent().commit();
   }

Beyond the simplest of applications, implicit Unit of Work management is more appropriate as it avoids repetitive, tedious coding. Here’s a servlet Layer Supertype (475) that registers and commits the Unit of Work for its concrete subtypes. Subtypes will implement handleGet() rather than override doGet(). Any code executing within handleGet() will have a Unit of Work with which to work.
Click here to view code image

class UnitOfWorkServlet...

   final protected void doGet(HttpServletRequest request, HttpServletResponse response)
         throws ServletException, IOException {
      try {
         UnitOfWork.newCurrent();
         handleGet(request, response);
         UnitOfWork.getCurrent().commit();
      } finally {
         UnitOfWork.setCurrent(null);
      }
   }
   abstract void handleGet(HttpServletRequest request, HttpServletResponse response)
         throws ServletException, IOException;