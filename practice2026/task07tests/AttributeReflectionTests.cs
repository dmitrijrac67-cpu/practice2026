using Xunit;
using System.Reflection;
using task07;

namespace task07tests
{
    public class AttributeReflectionTests
    {
        [Fact]
        public void Class_HasDisplayNameAttribute()
        {
            var type = typeof(SampleClass);
            var attr = type.GetCustomAttribute<DisplayNameAttribute>();
            Assert.NotNull(attr);
            Assert.Equal("Пример класса", attr.DisplayName);
        }

        [Fact]
        public void Method_HasDisplayNameAttribute()
        {
            var method = typeof(SampleClass).GetMethod("TestMethod")!;
            var attr = method.GetCustomAttribute<DisplayNameAttribute>();
            Assert.NotNull(attr);
            Assert.Equal("Тестовый метод", attr.DisplayName);
        }

        [Fact]
        public void Property_HasDisplayNameAttribute()
        {
            var prop = typeof(SampleClass).GetProperty("Number")!;
            var attr = prop.GetCustomAttribute<DisplayNameAttribute>();
            Assert.NotNull(attr);
            Assert.Equal("Числовое свойство", attr.DisplayName);
        }

        [Fact]
        public void Class_HasVersionAttribute()
        {
            var type = typeof(SampleClass);
            var attr = type.GetCustomAttribute<VersionAttribute>();
            Assert.NotNull(attr);
            Assert.Equal(1, attr.Major);
            Assert.Equal(0, attr.Minor);
        }
    }
}