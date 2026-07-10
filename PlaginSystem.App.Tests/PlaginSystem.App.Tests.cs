using System.Reflection;
using Xunit;
using CommandLib;

namespace PluginSystem.Tests
{
    [PluginLoad("Plugin1")]
    public class TestPlugin1 : ICommand
    {
        public void Execute() { }
    }

    [PluginLoad("Plugin2", PluginLoadPastNode = "Plugin1")]
    public class TestPlugin2 : ICommand
    {
        public void Execute() { }
    }

    public class PluginTests
    {
        [Fact]
        public void Test1_OnePlugin()
        {
            var attr = typeof(TestPlugin1).GetCustomAttribute<PluginLoadAttribute>();
            Assert.NotNull(attr);

            var instance = Activator.CreateInstance(typeof(TestPlugin1));
            Assert.IsAssignableFrom<ICommand>(instance);
        }

        [Fact]
        public void Test2_TwoPluginsWithDependency()
        {
            var attr = typeof(TestPlugin2).GetCustomAttribute<PluginLoadAttribute>();
            Assert.NotNull(attr);
            Assert.Equal("Plugin1", attr.PluginLoadPastNode);
        }

        [Fact]
        public void Test3_PluginInstanceCreation()
        {
            var plugin1 = Activator.CreateInstance(typeof(TestPlugin1)) as ICommand;
            var plugin2 = Activator.CreateInstance(typeof(TestPlugin2)) as ICommand;

            Assert.NotNull(plugin1);
            Assert.NotNull(plugin2);
            Assert.IsType<TestPlugin1>(plugin1);
            Assert.IsType<TestPlugin2>(plugin2);
        }
    }
}
