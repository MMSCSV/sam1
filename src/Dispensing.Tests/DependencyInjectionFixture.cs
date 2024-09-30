using System.Linq;
using CareFusion.Dispensing.DI;
using CareFusion.Dispensing.DI.Unity;
using NUnit.Framework;

namespace CareFusion.Dispensing.Tests
{
    [TestFixture(Category = "Unit")]
    public class DependencyInjectionFixture
    {
        [Test]
        public void MultipleRegistrationsTest()
        {
            IObjectFactory objectFactory = new UnityObjectFactory();

            objectFactory.RegisterType<IDog, Dog>("Dog1");
            objectFactory.RegisterType<IDog, Dog>("Dog2");
            objectFactory.RegisterType<IDog, Dog>("Dog3");

            var dogs = objectFactory.GetAll<IDog>();

            Assert.AreEqual(dogs.ToList().Count, 3);
        }

        [Test]
        public void ConstructorInjectionArrayTest()
        {
            IObjectFactory objectFactory = new UnityObjectFactory();

            objectFactory.RegisterType<IDog, Dog>();
            objectFactory.RegisterType<IDogFood, DogFood>("DogFood1");
            objectFactory.RegisterType<IDogFood, DogFood>("DogFood2");
            objectFactory.RegisterType<IDogFood, DogFood>("DogFood3");

            var dog = objectFactory.Get<IDog>();

            Assert.AreEqual(dog.DogFoodArray.Count(), 3);
        }

        [Test]
        public void ConstructorInjectionArrayUnityTest()
        {
            // This test was used to test differences between the default implementation of unity and our wrapper

            //var objectFactory = new Microsoft.Practices.Unity.UnityContainer();

            //objectFactory.RegisterType<IDog, Dog>();
            //objectFactory.RegisterType<IDogFood, DogFood>("DogFood1");
            //objectFactory.RegisterType<IDogFood, DogFood>("DogFood2");
            //objectFactory.RegisterType<IDogFood, DogFood>("DogFood3");

            //var dog = objectFactory.Resolve<IDog>();

            //Assert.AreEqual(dog.DogFoodArray.Count(), 3);
        }
    }

    public class Dog : IDog
    {
        public IDogFood[] DogFoodArray { get; set; }

        public Dog(IDogFood[] dogFoodArray)
        {
            DogFoodArray = dogFoodArray;
        }

    }

    public interface IDog
    {
        IDogFood[] DogFoodArray { get; set; }
    }

    public class DogFood : IDogFood
    {

    }

    public interface IDogFood
    {

    }

}
