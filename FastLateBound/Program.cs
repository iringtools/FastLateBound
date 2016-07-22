using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel.Composition.Hosting;

namespace FastLateBoundGenerics
{

    class Program
    {
        [Import]
        Lazy<IPerson> Person;

        [Import]
        Lazy<IDog> Dog;

        private readonly CompositionContainer _container;

        private Program()
        {
            AggregateCatalog catalog = new AggregateCatalog();
            DirectoryCatalog dirCatalog = new DirectoryCatalog("..\\..\\..\\Plugin\\bin\\Debug");
            catalog.Catalogs.Add(dirCatalog);
            _container = new CompositionContainer(catalog);

            try
            {
                _container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
                Console.ReadLine();
            }
        }

        static void Main(string[] args)
        {
            Program program = new Program();

            ////find unknown types and method by names
            //Type unknownPerson = Type.GetType("Plugin.Rob");
            //Type unknownDog = Type.GetType("Plugin.Shadow");
            //MethodInfo walkMethod = unknownPerson.GetMethod("Walk");
            //MethodInfo purchaseMethod = unknownPerson.GetMethod("Purchase");

            ////create and store delegates
            //Func<IDog> NewDogDelegate = InterfaceWrapper.MakeNewDelegate<IDog>(unknownDog);
            //Func<IPerson> NewPersonDelegate = InterfaceWrapper.MakeNewDelegate<IPerson>(unknownPerson);
            //Func<IPerson, IDog, IDog> walkDelegate = InterfaceWrapper.MakeCallDelegate<Func<IPerson, IDog, IDog>>(walkMethod);
            //Action<IPerson, IDog> purchaseDelegate = InterfaceWrapper.MakeCallDelegate<Action<IPerson, IDog>>(purchaseMethod);


            ////call delegates
            //IPerson personDelegate = NewPersonDelegate();
            //IDog dogDelegate = NewDogDelegate();
            //purchaseDelegate(personDelegate, dogDelegate);

            //benchmark
            IDog result;
            int times = 50000000;

            #region direct call 
            int i = times;
            IPerson person = new Library.Joe();
            IDog dog = new Library.Spot();
            DateTime start = DateTime.Now;

            while (i-- > 0)
                result = person.Walk(dog);

            Console.WriteLine("Direct call - {0}", (DateTime.Now - start).TotalMilliseconds);
            #endregion

            #region my way
            //i = times;
            //start = DateTime.Now;

            //while (i-- > 0)
            //    result = walkDelegate(personDelegate, dogDelegate);

            //Console.WriteLine("GenericDelegates - {0}", (DateTime.Now - start).TotalMilliseconds);
            #endregion

            #region Invoke
            //i = times;
            //object[] paramArray = new object[] { dog };
            //start = DateTime.Now;

            //while (i-- > 0)
            //    result = (IDog)walkMethod.Invoke(person, paramArray);

            //Console.WriteLine("Invoke - {0}", (DateTime.Now - start).TotalMilliseconds);
            #endregion

            #region MEF
            i = times;
            

            start = DateTime.Now;

            while (i-- > 0)
                dog = program.Dog.Value;

                result = program.Person.Value.Walk(dog);

            Console.WriteLine("MEF - {0}", (DateTime.Now - start).TotalMilliseconds);
            #endregion

            Console.ReadKey();
        }
    }

}