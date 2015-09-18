class PersonMapper...

   protected String findStatement() {
      return "SELECT " + COLUMNS +
         "  FROM people" +
         "  WHERE id= ?";
   }
   public static final String COLUMNS = " id, lastname, firstname, number_of_dependents ";
   public Person find(Long id) {
      return (Person) abstractFind(id);
   }
   public Person find(long id) {
      return find(new Long(id));
   }
class AbstractMapper...

   protected Map loadedMap = new HashMap();
   abstract protected String findStatement();
   protected DomainObject abstractFind(Long id) {
      DomainObject result = (DomainObject) loadedMap.get(id);
      if (result  != null) return result;
      PreparedStatement findStatement = null;
      try {
         findStatement = DB.prepare(findStatement());
         findStatement.setLong(1, id.longValue());
         ResultSet rs = findStatement.executeQuery();
         rs.next();
         result = load(rs);
         return result;
      } catch (SQLException e) {
         throw new ApplicationException(e);
      } finally {
         DB.cleanUp(findStatement);
      }
   }
   
   class AbstractMapper...

   protected DomainObject load(ResultSet rs) throws SQLException {
      Long id= new Long(rs.getLong(1));
      if (loadedMap.containsKey(id)) return (DomainObject) loadedMap.get(id);
      DomainObject result = doLoad(id, rs);
      loadedMap.put(id, result);
      return result;
   }
   abstract protected DomainObject doLoad(Long id, ResultSet rs) throws SQLException;

class PersonMapper...

   protected DomainObject doLoad(Long id, ResultSet rs) throws SQLException {
      String lastNameArg = rs.getString(2);
      String firstNameArg = rs.getString(3);
      int numDependentsArg = rs.getInt(4);
      return new Person(id, lastNameArg, firstNameArg, numDependentsArg);
   }
   
   
   class PersonMapper...

   private static String findLastNameStatement =
         "SELECT " + COLUMNS +
         "  FROM people " +
         "  WHERE UPPER(lastname) like UPPER(?)" +
         "  ORDER BY lastname";
   public List findByLastName(String name) {
      PreparedStatement stmt = null;
      ResultSet rs = null;
      try {
         stmt = DB.prepare(findLastNameStatement);
         stmt.setString(1, name);
         rs = stmt.executeQuery();
         return loadAll(rs);
      } catch (SQLException e) {
         throw new ApplicationException(e);
      } finally {
         DB.cleanUp(stmt, rs);
      }
   }

class AbstractMapper...

   protected List loadAll(ResultSet rs) throws SQLException {
      List result = new ArrayList();
      while (rs.next())
         result.add(load(rs));
      return result;
   }
   
   class AbstractMapper...

   public List findMany(StatementSource source) {
      PreparedStatement stmt = null;
      ResultSet rs = null;
      try {
         stmt = DB.prepare(source.sql());
         for (int i = 0; i < source.parameters().length;  i++)
            stmt.setObject(i+1, source.parameters()[i]);
         rs = stmt.executeQuery();
         return loadAll(rs);
      } catch (SQLException e) {
         throw new ApplicationException(e);
      } finally {
         DB.cleanUp(stmt, rs);
      }
   }
   
   class PersonMapper...

   public List findByLastName2(String pattern) {
      return findMany(new FindByLastName(pattern));
   }
   static class FindByLastName implements StatementSource {
      private String lastName;
      public FindByLastName(String lastName) {
         this.lastName = lastName;
      }
      public String sql() {
         return
            "SELECT " + COLUMNS +
            "  FROM people " +
            "  WHERE UPPER(lastname) like UPPER(?)" +
            "  ORDER BY lastname";
      }
      public Object[] parameters() {
         Object[] result = {lastName};
         return result;
      }
   }
   
   
   class PersonMapper...

   private static final String updateStatementString =
         "UPDATE people " +
         "  SET lastname = ?, firstname = ?, number_of_dependents = ? " +
         "  WHERE id= ?";
   public void update(Person subject) {
      PreparedStatement updateStatement = null;
      try {
         updateStatement = DB.prepare(updateStatementString);
         updateStatement.setString(1, subject.getLastName());
         updateStatement.setString(2, subject.getFirstName());
         updateStatement.setInt(3, subject.getNumberOfDependents());
         updateStatement.setInt(4, subject.getID().intValue());
         updateStatement.execute();
      } catch (Exception e) {
         throw new ApplicationException(e);
      } finally {
         DB.cleanUp(updateStatement);
      }
   }
   
   class AbstractMapper...

   public Long insert(DomainObject subject) {
      PreparedStatement insertStatement = null;
      try {
         insertStatement = DB.prepare(insertStatement());
         subject.setID(findNextDatabaseId());
         insertStatement.setInt(1, subject.getID().intValue());
         doInsert(subject, insertStatement);
         insertStatement.execute();
         loadedMap.put(subject.getID(), subject);
         return subject.getID();
      } catch (SQLException e) {
         throw new ApplicationException(e);
      } finally {
         DB.cleanUp(insertStatement);
      }
   }
   abstract protected String insertStatement();
   abstract protected void doInsert(DomainObject subject, PreparedStatement insertStatement)
         throws SQLException;

class PersonMapper...

   protected String insertStatement() {
      return  "INSERT INTO people VALUES (?, ?, ?, ?)";
   }
   protected void doInsert(
         DomainObject abstractSubject,
         PreparedStatement stmt)
         throws SQLException
   {
      Person subject = (Person) abstractSubject;
      stmt.setString(2, subject.getLastName());
      stmt.setString(3, subject.getFirstName());
      stmt.setInt(4, subject.getNumberOfDependents());
   }
   
   