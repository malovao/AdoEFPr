using System.Data.SqlClient;
using System.Collections.Generic;
using AdoEFPr;
using AdoEFPr.EF;
using static AdoEFPr.EF.DevTeamContext;

static void EFDemo()
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
        new Problem() {Title = "Создать базу данных", DateStart = new DateTime(2020, 1, 27), Person = persons[4]},
        new Problem() {Title = "Исправить верстку главной страницы", DateStart = new DateTime(2020, 1, 27), DateFinish = new DateTime(2020, 1, 22), Person = persons[3]},
        new Problem() {Title = "Провести тесты меню", DateStart = new DateTime(2020, 1, 17), DateFinish = new DateTime(2020, 1, 19), Person = persons[2]},
        new Problem() {Title = "Написать тесты для БД", DateStart = new DateTime(2020, 1, 27),  Person = persons[2]},
        new Problem() {Title = "Сделать авторизацию", DateStart = new DateTime(2020, 1, 19), DateFinish = new DateTime(2020, 1, 21), Person = persons[0]},
        new Problem() {Title = "Сделать чат", DateStart = new DateTime(2020, 1, 28), Person = persons[4]},
        new Problem() {Title = "Исправить оплату", DateStart = new DateTime(2020, 1, 23),Person = persons[3]},
        new Problem() {Title = "Добавить анимацию", DateStart = new DateTime(2020, 1, 23), DateFinish = new DateTime(2020, 1, 27), Person = persons[4]},
        new Problem() {Title = "Провести собрание", DateStart = new DateTime(2020, 1, 18), Person = persons[1]}
    };

   foreach(Problem p in problems)
    {
        db.Problems.Add(p);
    }
    db.SaveChanges();

    //1

    var doneAndNotDone = db.Problems.Where(pr => pr.DateFinish != null).OrderBy(pr => pr.DateFinish)
            .Union(db.Problems.Where(pr => pr.DateFinish == null).OrderBy(pr => pr.DateStart));

    foreach (var d in doneAndNotDone)
    {
        Console.WriteLine($"{d.Title} {d.DateStart} - {d?.DateFinish} {d.Person.Name}");
    }

    Console.WriteLine(" ");

    /*
    Console.WriteLine("1st:");
    foreach (Problem pr in db.Problems.Where(pr => pr.DateFinish == null).OrderBy(pr => pr.DateStart))
    {
        Console.WriteLine($"{pr.Title} {pr.DateStart}");
    }
    Console.WriteLine("2nd:");
    foreach (Problem pr in db.Problems.Where(pr => pr.DateFinish != null).OrderBy(pr => pr.DateFinish))
    {
        Console.WriteLine($"{pr.Title} {pr.DateStart} - {pr.DateFinish}");
    }
    */


    //3
    /*Console.WriteLine("3.Посчитать суммарное число задач выполненных каждой ролью в проекте:");

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
    */
}

/*static void EFDemo2()
{
    using (DevTeamContext db = new DevTeamContext())
    {
        Console.WriteLine("2.Посчитать процент выполненных задач (rate) каждым членом команды");

        var problemsDone = from per in db.Persons
                           join pr in db.Problems on per.Id equals pr.Person.Id into pg
                           from p in pg
                           where p.DateFinish != null
                           group p by p.Person.Name into gr
                           select new
                           {
                               gr.Key,
                               d = gr.Count()
                           };

        var problemsAll = from per in db.Persons
                          join pr in db.Problems on per.Id equals pr.Person.Id into pg
                          from p in pg
                          group p by p.Person.Name into gr
                          select new
                          {
                              r = gr.Count(),
                          };

        foreach (var p in problemsDone)
        {
            Console.WriteLine($"{p.Key} {p.d}");
        }

        foreach (var pr in problemsAll)
        {
            Console.WriteLine($"{pr.r}");
        }

        Console.WriteLine(" ");
    }
}*/

static void AdoDemo2()
{
    Console.WriteLine("2.Посчитать процент выполненных задач (rate) каждым членом команды:");

    var connection = new SqlConnection(@"Server=.\SQLEXPRESS;Database=DevTeam;Trusted_Connection=True;");
    connection.Open();

    var command = connection.CreateCommand();
    command.CommandText = "SELECT per.Name, ((COUNT(DateFinish))*100/(COUNT(PersonId))) AS Amount FROM Persons per LEFT JOIN Problems pr ON per.Id = pr.PersonId GROUP BY per.Name; ";
    var reader = command.ExecuteReader();

    while (reader.Read())
    {
        Console.WriteLine($"Role = {reader.GetString(reader.GetOrdinal("Name"))}   Rate = {reader.GetInt32(1)}%");
    }

    reader.Close();
    connection.Close();

    Console.WriteLine(" ");
}

static void EFDemo3()
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

/*static void EFDemo4()
{
    using (DevTeamContext db = new DevTeamContext())
    {
        Console.WriteLine("4.Удалить участника с наим. rate (не руководителя), заверш. задачи передать руководителю, " +
            "незаверш. - участнику с наиб. rate");
        
        var mostRated = from per in db.Persons
                        join pr in db.Problems on per.Id equals pr.Person.Id
                        group pr by pr.DateFinish into result
                        
    }

}*/


EFDemo();
AdoDemo2();
//EFDemo2();
EFDemo3();
//EFDemo4();
