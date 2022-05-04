using System.Data.SqlClient;
using System.Collections.Generic;
using AdoEFPr;
using AdoEFPr.EF;
using static AdoEFPr.EF.DevTeamContext;

static void EntFr()
{
    DevTeamContext db = new DevTeamContext();
    db.CreateDbIfNotExist();

    var roles = new List<Role>
    {
        new Role() {Title = "Тестировщик" },
        new Role() {Title = "Программист" },
        new Role() {Title = "Руководитель проекта" }
    };

    foreach (Role r in roles)
    {
        db.Roles.Add(r);
    }
    db.SaveChanges();


    var persons = new List<Person>
    {
        new Person() {Name = "Дмитрий", Role = roles[1]},
        new Person() {Name = "Евгений", Role = roles[2]},
        new Person() {Name = "Анна", Role = roles[0]},
        new Person() {Name = "София", Role = roles[1]},
        new Person() {Name = "Фёдор", Role = roles[1]},
    };

    foreach (Person p in persons)
    {
        db.Persons.Add(p);
    }

    db.SaveChanges();


    var problems = new List<Problem>
    {
        new Problem() {Title = "Сделать меню", DateStart = new DateTime(2020, 1, 10), DateFinish = new DateTime(2020, 1, 13), Person = persons[0]},
        new Problem() {Title = "Создать базу данных", DateStart = new DateTime(2020, 1, 27), /*DateFinish = new DateTime(),*/ Person = persons[4]},
        new Problem() {Title = "Исправить верстку главной страницы", DateStart = new DateTime(2020, 1, 27), DateFinish = new DateTime(2020, 1, 22), Person = persons[3]},
        new Problem() {Title = "Провести тесты меню", DateStart = new DateTime(2020, 1, 17), DateFinish = new DateTime(2020, 1, 19), Person = persons[2]},
        new Problem() {Title = "Написать тесты для БД", DateStart = new DateTime(2020, 1, 27), /*DateFinish = new DateTime(),*/ Person = persons[2]},
        new Problem() {Title = "Сделать авторизацию", DateStart = new DateTime(2020, 1, 19), DateFinish = new DateTime(2020, 1, 21), Person = persons[0]},
        new Problem() {Title = "Сделать чат", DateStart = new DateTime(2020, 1, 28), /*DateFinish = new DateTime(),*/ Person = persons[4]},
        new Problem() {Title = "Исправить оплату", DateStart = new DateTime(2020, 1, 23), /*DateFinish = new DateTime(),*/ Person = persons[3]},
        new Problem() {Title = "Добавить анимацию", DateStart = new DateTime(2020, 1, 23), DateFinish = new DateTime(2020, 1, 27), Person = persons[4]},
        new Problem() {Title = "Провести собрание", DateStart = new DateTime(2020, 1, 18), /*DateFinish = new DateTime(),*/ Person = persons[1]}
    };

   foreach(Problem p in problems)
    {
        db.Problems.Add(p);
    }
    db.SaveChanges();

    Console.WriteLine("1.Список завершенных задач по дате выполнения и незавершенных по дате постановки:");

    var doneAndNotDone = db.Problems.Where(pr => pr.DateFinish != null).OrderBy(pr => pr.DateFinish)
            .Union(db.Problems.Where(pr => pr.DateFinish == null).OrderBy(pr => pr.DateStart));

    foreach (var d in doneAndNotDone)
    {
        Console.WriteLine($"{d.Title} {d.DateStart} - {d.DateFinish} {d.Person.Name}");
    }

    Console.WriteLine(" ");

}

static void EntFr2()
{
    using (DevTeamContext db = new DevTeamContext())
    {
        Console.WriteLine("2.Посчитать процент выполненных задач (rate) каждым членом команды");

        var personRate = from per in db.Persons
                          join pr in db.Problems on per.Id equals pr.Person.Id into pg
                          from p in pg
                          group p by p.Person.Name into gr
                          select new
                          {
                              name = gr.Key,
                              rate = gr.Count(x => x.DateFinish != null)*100/gr.Count(),
                          };

        foreach (var p in personRate)
        {
            Console.WriteLine($"{p.name} - {p.rate}%");
        }
    }
}

static void EntFr3()
{
    using (DevTeamContext db = new DevTeamContext())
    {
        Console.WriteLine("3.Посчитать суммарное число задач выполненных каждой ролью в проекте:");

        var problemsRolesDone = from pers in db.Persons
                                join prob in db.Problems on pers.Id equals prob.Person.Id
                                where prob.DateFinish != null
                                join role in db.Roles on pers.Role.Id equals role.Id into gr
                                from g in gr
                                group g by g.Title into result
                                select new
                                {
                                    result.Key,
                                    amount = result.Count(),
                                };

        foreach (var pRD in problemsRolesDone)
        {
            Console.WriteLine($"{pRD.Key} {pRD.amount}");
        }

        Console.WriteLine(" ");
    }
}

static void EntFr4()
{
    using (DevTeamContext db = new DevTeamContext())
    {
        Console.WriteLine("4.Удалить участника с наим. rate (не руководителя), заверш. задачи передать руководителю, " +
            "незаверш. - участнику с наиб. rate");

        var personRate = from per in db.Persons
                         join pr in db.Problems on per.Id equals pr.Person.Id into pg
                         from p in pg
                         group p by p.Person.Name into gr
                         select new
                         {
                             name = gr.Key,
                             rate = gr.Count(x => x.DateFinish != null) * 100 / gr.Count(),
                         };

        var lessRated = personRate.Where(x => x.name != "Евгений").OrderBy(x => x.rate).First();
        db.Remove(lessRated);
        db.SaveChanges();

        Console.WriteLine("Done!");
    }
}

static void AdoNet1()
{

    Console.WriteLine("1.Список завершенных задач по дате выполнения и незавершенных по дате постановки:");

    var connection = new SqlConnection(@"Server=.\SQLEXPRESS;Database=DevTeam;Trusted_Connection=True;");
    connection.Open();


    var command = connection.CreateCommand();
    command.CommandText = "SELECT Title, DateStart, DateFinish FROM Problems UNION SELECT Title, DateStart, DateFinish FROM Problems WHERE DateFinish != '' ORDER BY DateFinish; ";
    var reader = command.ExecuteReader();

    while (reader.Read())
    {
        Console.WriteLine($"Role={reader.GetString(reader.GetOrdinal("Title"))} DateStart={reader.GetDateTime(reader.GetOrdinal("DateStart"))} DateFinish ={reader.GetDateTime(reader.GetOrdinal("DateFinish"))}");
    }

    reader.Close();
    connection.Close();
}

static void AdoNet2()
{
    Console.WriteLine("2.Посчитать процент выполненных задач (rate) каждым членом команды:");

    var connection = new SqlConnection(@"Server=.\SQLEXPRESS;Database=DevTeam;Trusted_Connection=True;");
    connection.Open();

    var command = connection.CreateCommand();
    command.CommandText = "SELECT per.Name, ((COUNT(DateFinish))*100/(COUNT(PersonId))) AS Amount FROM Persons per LEFT JOIN Problems pr ON per.Id = pr.PersonId GROUP BY per.Name; ";
    var reader = command.ExecuteReader();

    while (reader.Read())
    {
        Console.WriteLine($"{reader.GetString(reader.GetOrdinal("Name"))}   {reader.GetInt32(1)}%");
    }

    reader.Close();
    connection.Close();

    Console.WriteLine(" ");
}

static void AdoNet3()

{

    Console.WriteLine("3.Посчитать суммарное число задач выполненных каждой ролью в проекте:");

    var connection = new SqlConnection(@"Server=.\SQLEXPRESS;Database=DevTeam;Trusted_Connection=True;");
    connection.Open();


    var command = connection.CreateCommand();
    command.CommandText = "SELECT r.Title, COUNT(pr.DateFinish) Amount  FROM Persons per LEFT JOIN Problems pr ON per.Id = pr.PersonId LEFT JOIN Roles r ON per.RoleId = r.Id GROUP BY r.Title; ";
    var reader = command.ExecuteReader();

    while (reader.Read())
    {
        Console.WriteLine($"{reader.GetString(reader.GetOrdinal("Title"))}  {reader.GetInt32(1)}");
    }

    reader.Close();
    connection.Close();
}

static void AdoNet4()
{
    Console.WriteLine("4.Удалить участника с наим. rate (не руководителя), заверш. задачи передать руководителю, " +
            "незаверш. - участнику с наиб. rate");

    var connection = new SqlConnection(@"Server=.\SQLEXPRESS;Database=DevTeam;Trusted_Connection=True;");
    connection.Open();

    var command = connection.CreateCommand();
    command.CommandText = "CREATE VIEW view_pers AS SELECT per.Name, ((COUNT(DateFinish))*100/(COUNT(PersonId))) AS Amount FROM Persons per LEFT JOIN Problems pr ON per.Id = pr.PersonId GROUP BY per.Name; ";
    var reader = command.ExecuteReader();

    var command2 = connection.CreateCommand();
    command2.CommandText = "DELETE FROM Persons WHERE Persons.Name = (SELECT Name FROM view_pers WHERE Amount = (SELECT MIN(Amount) FROM view_pers) ) AND Persons.RoleId != 3;";
    var reader2 = command.ExecuteReader();
    reader2.Close();

    Console.WriteLine("Done!");

    connection.Close();
}


//EntFr();
EntFr2();
EntFr3();
EntFr4();
//AdoNet1();
AdoNet2();
AdoNet3();
AdoNet4();