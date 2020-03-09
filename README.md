# Simple attempt to realize EF Core Expression serialization/deserialization and restoring EF Core Expression for remote query execution.
Use SimpleInjector as a DI Container.

Additional classes:

public static class InvokeExtensionMethods
{
  public static async Task<object> DynamicInvokeAsync(this MethodInfo @this, object obj, params object[] parameters)
  {
    dynamic awaitable = @this.Invoke(obj, parameters);
    await awaitable;
    return awaitable.GetAwaiter().GetResult();
  }

  public static async Task<object> InvokeAsync(this MethodInfo @this, object obj, params object[] parameters)
  {
    var task = (Task)@this.Invoke(obj, parameters);
    await task.ConfigureAwait(false);
    var resultProperty = task.GetType().GetProperty("Result");
    return resultProperty.GetValue(task);
  }
}

public class GuidShortGuidConverter : JsonConverter
{
  public override bool CanConvert(Type objectType)
  {
    return objectType == typeof(Guid);
  }

  public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
  {
    var shortGuid = Guid.Parse(reader.Value.ToString());//new ShortGuid(reader.Value.ToString());
    return shortGuid;
  }

  public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
  {
    writer.WriteValue(((Guid)value).ToString("D"));
  }
}


Usage:

var container = new Container();
var expressionSettings = new ExpressionParserSettings()
{
  Container = container,
  StoringKnownTypes = false,
};
var queryableExpressionSettings = new QueryableExpressionParserSettings()
{
  Container = container,
  StoringKnownTypes = false,
};
var memberBindingSettings = new MemberBindingParserSettings()
{
  Container = container,
  StoringKnownTypes = false,
};
var memberInfoSettings = new MemberInfoParserSettings()
{
  Container = container,
  StoringKnownTypes = false,
};
var typeSettings = new TypeParserSettings()
{
  Container = container,
  StoringKnownTypes = false,
};

var providerAsync = testDb.GetService<IAsyncQueryProvider>(); //Here testDb is a same DbContext.

container.RegisterSingleton<IExpressionParser>(() => new QueryableExpressionParser(queryableExpressionSettings));
container.RegisterSingleton<IMemberBingingParser>(() => new BaseMemberBindingParser(memberBindingSettings));
container.RegisterSingleton<IMemberInfoParser>(() => new BaseMemberInfoParser(memberInfoSettings));
container.RegisterSingleton<ITypeParser>(() => new BaseTypeParser(typeSettings));
container.RegisterSingleton<IQueryProvider>(() => providerAsync);

var dataQuery = testDb.Users.Include(e => e.EmployeeInfo).Include(f => f.Notifications).Where(k => tstList.Contains(k.Id)).Select(e => e.FullName); // s.Id == valtst
var tstEspressionBase = dataQuery.Expression;
var providerAsync = testDb.GetService<IAsyncQueryProvider>();
var tstEspression = new ReflectionLocalCalculationVisitor().Visit(tstEspressionBase);


var jsonSerializerSettings = new JsonSerializerSettings()
{
  CheckAdditionalContent = true,
  ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
  TypeNameHandling = TypeNameHandling.All,
  TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
  StringEscapeHandling = StringEscapeHandling.Default,
  ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
  PreserveReferencesHandling = PreserveReferencesHandling.All,
  ObjectCreationHandling = ObjectCreationHandling.Replace,
  NullValueHandling = NullValueHandling.Include,
  MissingMemberHandling = MissingMemberHandling.Ignore,
  MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
  MaxDepth = int.MaxValue,
  FloatParseHandling = FloatParseHandling.Double,
  DateParseHandling = DateParseHandling.DateTime,
  DateFormatHandling = DateFormatHandling.IsoDateFormat,
  DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
  DateTimeZoneHandling = DateTimeZoneHandling.Utc,
  FloatFormatHandling = FloatFormatHandling.DefaultValue,                
};

jsonSerializerSettings.Converters.Add(new GuidShortGuidConverter());
jsonSerializerSettings.Error += (sender, e) => Console.WriteLine("{0}   {1}", e.ErrorContext.Error.Message, e.ErrorContext.Path);

var newtonsoftSerialized = JsonConvert.SerializeObject(result, jsonSerializerSettings);
var newtonsoftDeserialized = JsonConvert.DeserializeObject(newtonsoftSerialized, jsonSerializerSettings) as ExpressionNode;

var generatedDeserializedExpression = newtonsoftDeserialized.FromNode(container);

var toListAsyncMethodInfo = typeof(EntityFrameworkQueryableExtensions).GetMethod(nameof(EntityFrameworkQueryableExtensions.ToListAsync)).MakeGenericMethod(typeof(string));
var restoredResult = await toListAsyncMethodInfo.InvokeAsync(null, new object[] { providerAsync.CreateQuery(generatedDeserializedExpression), default(CancellationToken) }).ConfigureAwait(false);

It's an alpha version, it still doesn't work properly.

