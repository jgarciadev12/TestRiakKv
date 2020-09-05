using RiakClient;
using RiakClient.Extensions;
using RiakClient.Models;
using RiakClient.Models.Index;
using RiakClient.Models.Search;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestRiak.EjCustomer;
using TestRiak.Entity;
using TestRiak.ViewModel;

namespace TestRiak
{
    public partial class Form1 : Form
    {
        HotelEntities db = new HotelEntities();
        IRiakClient client;

        const string precioBucketName = "Precio";
        const string formaPagoBucketName = "FormaPago";
        const string clienteBucketName = "Cliente";
        const string habitacionBucketName = "Habitacion";
        const string facturasBucketName = "Factura";
        const string empleadoBucketName = "Empleado";

        const string habitacionTipoIndexName = "HabitacionTipo";

        const string facturaFacturaEntradaDateIndexName = "FacturaEntradaDate";
        const string facturaFacturaSalidaDateIndexName = "FacturaSalidaDate";
        const string facturaFacturaDNIIndexName = "FacturaDNI";
        const string facturaFacturaNumeroIndexName = "FacturaNumero";
        const string facturaFacturaFormaIndexName = "FacturaForma";
        const string facturaFacturaTotalIndexName = "FacturaTotal";

        const string empleadoNombreIndexName = "EmpleadoNombre";
        const string empleadoServicioDescripcionIndexName = "EmpleadoSerDescripcion";
        const string empleadoCodSIndexName = "EmpleadoCodS";



        //ejemplo
        const string customersBucketName = "Customers";

        const string ordersBucketName = "Orders";
        const string ordersSalesPersonIdIndexName = "SalesPersonId";
        const string ordersOrderDateIndexName = "OrderDate";

        const string orderSummariesBucketName = "OrderSummaries";
        //eejmplo

        public Form1()
        {
            InitializeComponent();
            IRiakEndPoint cluster = RiakCluster.FromConfig("riakConfig");
            client = cluster.CreateClient();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Cargar Datos
            Console.WriteLine("Loading Data FormaPago");
            List<FormaPagoVM> formaPagoVMs = db.FormaPago.Select(b => new FormaPagoVM { Forma = b.Forma, comision = b.comision }).ToList();
            Console.WriteLine("Loading Data Precio");
            List<PrecioVM> precioVMs = db.Precio.Select(a => new PrecioVM { Tipo = a.Tipo, precio = a.precio1 }).ToList();
            Console.WriteLine("Loading Data Cliente");
            List<ClienteVM> clienteVMs = db.Cliente.Select(a => new ClienteVM { DNI = a.DNI, Nombre = a.Nombre, Apellidos = a.Apellidos, Domicilio = a.Domicilio, Telefono = a.Telefono }).ToList();
            Console.WriteLine("Loading Data Habitacion");
            List<HabitacionVM> habitacionVMs = db.Habitacion.Select(b => new HabitacionVM 
            { Numero = b.Numero, bar = b.bar, puedesupletoria = b.puedesupletoria, superficie = b.superficie, terraza = b.terraza, 
                Tipo = new PrecioVM { Tipo = b.Precio.Tipo, precio = b.Precio.precio1 }
            }).ToList();

            Console.WriteLine("Loading Data Factura");
            List<FacturaVM> facturaVMs = db.Factura.Select(a =>
            new FacturaVM
            {
                CodigoF = a.CodigoF,
                Entrada = a.Entrada == null ? DateTime.MinValue : (DateTime)a.Entrada,
                Salida = a.Salida == null ? DateTime.MinValue : (DateTime)a.Salida,
                Cliente = new ClienteVM
                {
                    DNI = a.Cliente.DNI,
                    Nombre = a.Cliente.Nombre,
                    Apellidos = a.Cliente.Apellidos,
                    Domicilio = a.Cliente.Domicilio,
                    Telefono = a.Cliente.Telefono
                },
                Numero = new HabitacionVM
                {
                    Numero = a.Habitacion.Numero,
                    superficie = a.Habitacion.superficie,
                    bar = a.Habitacion.bar,
                    puedesupletoria = a.Habitacion.puedesupletoria,
                    terraza = a.Habitacion.terraza,
                    Tipo = new PrecioVM { Tipo = a.Habitacion.Precio.Tipo, precio = a.Habitacion.Precio.precio1 }
                },
                Forma = new FormaPagoVM { Forma = a.FormaPago.Forma, comision = a.FormaPago.comision }
            }).ToList();

            Console.WriteLine("Loading Data Empleado");

            List<EmpleadoVM> empleadoVMs = new List<EmpleadoVM>();

            foreach (Empleado a in db.Empleado)
            {
                EmpleadoVM empleadoVM = new EmpleadoVM();
                empleadoVM.NumReg = a.NumReg;
                empleadoVM.Nombre = a.Nombre;
                empleadoVM.Incorporacion = a.Incorporacion == null ? DateTime.MinValue : (DateTime)a.Incorporacion;
                empleadoVM.Sueldo = a.Sueldo == null ? 0 : (int)a.Sueldo;

                if (a.Servicio != null)
                {
                    ServicioVM servicioVM = new ServicioVM();
                    servicioVM.CodS = a.Servicio.CodS;
                    servicioVM.Descripcion = a.Servicio.Descripcion.Trim();
                    servicioVM.costeinterno = a.Servicio.costeinterno == null ? 0 : (int)a.Servicio.costeinterno;

                    if (a.Servicio.Empleado1 != null)
                    {
                        EmpleadoVM empleadoVM1 = new EmpleadoVM();
                        empleadoVM1.NumReg = a.Servicio.Empleado1.NumReg;
                        empleadoVM1.Nombre = a.Servicio.Empleado1.Nombre;
                        empleadoVM1.Incorporacion = a.Servicio.Empleado1.Incorporacion == null ? DateTime.MinValue : (DateTime)a.Servicio.Empleado1.Incorporacion;
                        empleadoVM1.Sueldo = a.Servicio.Empleado1.Sueldo == null ? 0 : (int)a.Servicio.Empleado1.Sueldo;

                        servicioVM.NumReg = empleadoVM1;
                    }

                    empleadoVM.Servicio = servicioVM;
                }

                empleadoVMs.Add(empleadoVM);
            }

            foreach (FormaPagoVM itemFormaPagoVM in formaPagoVMs)
            {
                client.Put(ToRiakObject(itemFormaPagoVM));
            }

            foreach (PrecioVM itemPrecioVM in precioVMs)
            {
                client.Put(ToRiakObject(itemPrecioVM));
            }

            foreach (ClienteVM itemClienteVM in clienteVMs)
            {
                client.Put(ToRiakObject(itemClienteVM));
            }

            foreach (FacturaVM itemFacturaVM in facturaVMs)
            {
                client.Put(ToRiakObject(itemFacturaVM));
            }


            foreach (HabitacionVM itemHabitacionVM in habitacionVMs)
            {
                client.Put(ToRiakObject(itemHabitacionVM));
            }

            foreach (EmpleadoVM itemEmpleadoVM in empleadoVMs)
            {
                client.Put(ToRiakObject(itemEmpleadoVM));
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var pingResult = client.Ping();

            if (pingResult.IsSuccess)
            {
                Console.WriteLine("pong");
            }
            else
            {
                Console.WriteLine("Are you sure Riak is running?");
                Console.WriteLine("{0}: {1}", pingResult.ResultCode, pingResult.ErrorMessage);
            }

        }

        private static RiakObject ToRiakObject(PrecioVM precioVM)
        {
            var precioRiakObjectId = new RiakObjectId(precioBucketName, precioVM.Tipo);
            return new RiakObject(precioRiakObjectId, precioVM);
        }

        private static RiakObject ToRiakObject(FormaPagoVM formaPagoVM)
        {
            var formaPagoRiakObjectId = new RiakObjectId(formaPagoBucketName, formaPagoVM.Forma);
            return new RiakObject(formaPagoRiakObjectId, formaPagoVM);
        }


        private static RiakObject ToRiakObject(ClienteVM clienteVM)
        {
            var clienteRiakObjectId = new RiakObjectId(clienteBucketName, clienteVM.DNI);
            return new RiakObject(clienteRiakObjectId, clienteVM);
        }

        private static RiakObject ToRiakObject(HabitacionVM habitacionVM)
        {
            var habitacionRiakObjectId = new RiakObjectId(habitacionBucketName, habitacionVM.Numero.ToString());
            var riakObject = new RiakObject(habitacionRiakObjectId, habitacionVM);

            BinIndex habitacionTipoIndex = riakObject.BinIndex(habitacionTipoIndexName);
            habitacionTipoIndex.Add(habitacionVM.Tipo.Tipo);

            return riakObject;
        }



        private static RiakObject ToRiakObject(FacturaVM facturaVM)
        {
            var facturaRiakObjectId = new RiakObjectId(facturasBucketName, facturaVM.CodigoF.ToString());
            var riakObject = new RiakObject(facturaRiakObjectId, facturaVM);

            BinIndex facturaEntradaDateIndex = riakObject.BinIndex(facturaFacturaEntradaDateIndexName);
            facturaEntradaDateIndex.Add(facturaVM.Entrada.ToString("yyyy-MM-dd"));

            BinIndex facturaSalidaDateIndex = riakObject.BinIndex(facturaFacturaSalidaDateIndexName);
            facturaSalidaDateIndex.Add(facturaVM.Salida.ToString("yyyy-MM-dd"));

            BinIndex facturaDNIDateIndex = riakObject.BinIndex(facturaFacturaDNIIndexName);
            facturaDNIDateIndex.Add(facturaVM.Cliente.DNI);

            BinIndex facturaNumeroDateIndex = riakObject.BinIndex(facturaFacturaNumeroIndexName);
            facturaNumeroDateIndex.Add(facturaVM.Numero.Numero.ToString());

            BinIndex facturaFormaDateIndex = riakObject.BinIndex(facturaFacturaFormaIndexName);
            facturaFormaDateIndex.Add(facturaVM.Forma.Forma.ToString());

            BinIndex facturaTotalDateIndex = riakObject.BinIndex(facturaFacturaTotalIndexName);
            facturaTotalDateIndex.Add(facturaVM.Total.ToString());


            return riakObject;
        }




        private static RiakObject ToRiakObject(EmpleadoVM empleadoVM)
        {

            var empleadoRiakObjectId = new RiakObjectId(empleadoBucketName, empleadoVM.NumReg.ToString());
            var riakObject = new RiakObject(empleadoRiakObjectId, empleadoVM);

            BinIndex empleadoNombreIndex = riakObject.BinIndex(empleadoNombreIndexName);
            empleadoNombreIndex.Add(empleadoVM.Nombre);

            if (empleadoVM.Servicio != null)
            {
                BinIndex empleadoSerDescIndex = riakObject.BinIndex(empleadoServicioDescripcionIndexName);
                empleadoSerDescIndex.Add(empleadoVM.Servicio.Descripcion);

                IntIndex empleadoCodSIndex = riakObject.IntIndex(empleadoCodSIndexName);
                empleadoCodSIndex.Add(empleadoVM.Servicio.CodS.ToString());
            }

            return riakObject;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var idx = new SearchIndex("famous");
            var rslt = client.PutSearchIndex(idx);

            var properties = new RiakBucketProperties();
            properties.SetSearchIndex("famous");
            var rslt1 = client.SetBucketProperties("cats", properties);


            idx = new SearchIndex(empleadoCodSIndexName);
            rslt = client.PutSearchIndex(idx);

            properties = new RiakBucketProperties();
            properties.SetSearchIndex(empleadoCodSIndexName);
            rslt1 = client.SetBucketProperties(empleadoBucketName, properties);


        }

        private void button4_Click(object sender, EventArgs e)
        {
            var lionoId = new RiakObjectId("cats", "liono");
            var lionoObj = new { name_s = "Lion-o", age_i = 30, leader = true };
            var lionoRiakObj = new RiakObject(lionoId, lionoObj);

            var cheetaraId = new RiakObjectId("cats", "cheetara");
            var cheetaraObj = new { name_s = "Cheetara", age_i = 30, leader = false };
            var cheetaraRiakObj = new RiakObject(cheetaraId, cheetaraObj);

            var snarfId = new RiakObjectId("cats", "snarf");
            var snarfObj = new { name_s = "Snarf", age_i = 43, leader = false };
            var snarfRiakObj = new RiakObject(snarfId, snarfObj);

            var panthroId = new RiakObjectId( "cats", "panthro");
            var panthroObj = new { name_s = "Panthro", age_i = 36, leader = false };
            var panthroRiakObj = new RiakObject(panthroId, panthroObj);

            var rslts = client.Put(new[] {
             lionoRiakObj, cheetaraRiakObj, snarfRiakObj, panthroRiakObj
            });

        }

        private void button5_Click(object sender, EventArgs e)
        {
            var search = new RiakSearchRequest
            {
                Query = new RiakFluentSearch("famous", "name_s")
        .Search("Lion*")
        .Build()
            };

            var rslt = client.Search(search);
            RiakSearchResult searchResult = rslt.Value;
            foreach (RiakSearchResultDocument doc in searchResult.Documents)
            {
                var args = new[] {
        doc.BucketType,
        doc.Bucket,
        doc.Key,
        string.Join(", ", doc.Fields.Select(f => f.Value).ToArray())
        };
                Console.WriteLine(
                    format: "BucketType: {0} Bucket: {1} Key: {2} Values: {3}",
                    args);
            }


            var search1 = new RiakSearchRequest("famous", "age_i:[36 TO *]");
            var rslt1 = client.Search(search1);

        }

        private void button7_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Creating Data");
            Customer customer = CreateCustomer();
            IEnumerable<Order> orders = CreateOrders(customer);
            OrderSummary orderSummary = CreateOrderSummary(customer, orders);
            Console.WriteLine("Storing Data");

            client.Put(ToRiakObject(customer));

            foreach (Order order in orders)
            {
                // NB: this adds secondary index data as well
                client.Put(ToRiakObject(order));
            }

            client.Put(ToRiakObject(orderSummary));
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Fetching related data by shared key");
            string key = "1";

            var result = client.Get(customersBucketName, key);
            CheckResult(result);
            Console.WriteLine("Customer     1: {0}\n", GetValueAsString(result));

            result = client.Get(orderSummariesBucketName, key);
            CheckResult(result);
            Console.WriteLine("OrderSummary 1: {0}\n", GetValueAsString(result));
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Index Queries");

            // Query for order keys where the SalesPersonId index is set to 9000
            var riakIndexId = new RiakIndexId(ordersBucketName, ordersSalesPersonIdIndexName);
            RiakResult<RiakIndexResult> indexRiakResult = client.GetSecondaryIndex(riakIndexId, 9000); // NB: *must* use 9000 as integer here.
            CheckResult(indexRiakResult);
            RiakIndexResult indexResult = indexRiakResult.Value;
            Console.WriteLine("Jane's orders (key values): {0}", string.Join(", ", indexResult.IndexKeyTerms.Select(ikt => ikt.Key)));

            // Query for orders where the OrderDate index is between 2013-10-01 and 2013-10-31
            riakIndexId = new RiakIndexId(ordersBucketName, ordersOrderDateIndexName);
            indexRiakResult = client.GetSecondaryIndex(riakIndexId, "2013-10-01", "2013-10-31"); // NB: *must* use strings here.
            CheckResult(indexRiakResult);
            indexResult = indexRiakResult.Value;
            Console.WriteLine("October orders (key values): {0}", string.Join(", ", indexResult.IndexKeyTerms.Select(ikt => ikt.Key)));

        }

        private static string GetValueAsString(RiakResult<RiakObject> result)
        {
            string rv = string.Empty;

            if (result.IsSuccess)
            {
                RiakObject riakObject = result.Value;
                rv = Encoding.UTF8.GetString(riakObject.Value);
            }
            else
            {
                rv = "ERROR";
            }

            return rv;
        }

        private static void CheckResult<T>(RiakResult<T> result)
        {
            if (!result.IsSuccess)
            {
                Console.Error.WriteLine("Error: {0}", result.ErrorMessage);
                Environment.Exit(1);
            }
        }

        private static RiakObject ToRiakObject(Customer customer)
        {
            var customerRiakObjectId = new RiakObjectId(customersBucketName, customer.Id.ToString());
            return new RiakObject(customerRiakObjectId, customer);
        }

        private static RiakObject ToRiakObject(Order order)
        {
            var orderRiakObjectId = new RiakObjectId(ordersBucketName, order.Id.ToString());
            var riakObject = new RiakObject(orderRiakObjectId, order);

            IntIndex salesPersonIdIndex = riakObject.IntIndex(ordersSalesPersonIdIndexName);
            salesPersonIdIndex.Add(order.SalesPersonId.ToString());

            BinIndex orderDateIndex = riakObject.BinIndex(ordersOrderDateIndexName);
            orderDateIndex.Add(order.OrderDate.ToString("yyyy-MM-dd"));

            return riakObject;
        }

        private static RiakObject ToRiakObject(OrderSummary orderSummary)
        {
            var orderSummaryRiakObjectId = new RiakObjectId(orderSummariesBucketName, orderSummary.CustomerId.ToString());
            return new RiakObject(orderSummaryRiakObjectId, orderSummary);
        }

        public static Customer CreateCustomer()
        {
            return new Customer(
                id: 1,
                name: "John Smith",
                address: "123 Main Street",
                city: "Columbus",
                state: "Ohio",
                zip: "43210",
                phone: "+1-614-555-5555",
                createdDate: DateTime.Parse("2013-10-01 14:30:26")
            );
        }

        private static IEnumerable<Order> CreateOrders(Customer customer)
        {
            var orders = new List<Order>();

            var order1 = new Order(
                id: 1,
                customerId: customer.Id,
                salesPersonId: 9000,
                orderDate: DateTime.Parse("2013-10-01 14:42:26")
                );
            order1.AddItem(new Item("TCV37GIT4NJ", "USB 3.0 Coffee Warmer", 15.99m));
            order1.AddItem(new Item("PEG10BBF2PP", "eTablet Pro; 24GB; Grey", 399.99m));
            orders.Add(order1);

            var order2 = new Order(
                id: 2,
                customerId: customer.Id,
                salesPersonId: 9001,
                orderDate: DateTime.Parse("2013-10-15 16:43:16")
            );
            order2.AddItem(new Item("OAX19XWN0QP", "GoSlo Digital Camera", 359.99m));
            orders.Add(order2);

            var order3 = new Order(
                id: 3,
                customerId: customer.Id,
                salesPersonId: 9000,
                orderDate: DateTime.Parse("2013-11-03 17:45:28")
            );
            order3.AddItem(new Item("WYK12EPU5EZ", "Call of Battle = Goats - Gamesphere 4", 69.99m));
            order3.AddItem(new Item("TJB84HAA8OA", "Bricko Building Blocks", 4.99m));
            orders.Add(order3);

            return orders;
        }

        private static OrderSummary CreateOrderSummary(Customer customer, IEnumerable<Order> orders)
        {
            var orderSummary = new OrderSummary(customerId: customer.Id);

            foreach (Order order in orders)
            {
                orderSummary.AddSummaryItem(new OrderSummaryItem(order));
            }

            return orderSummary;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //1) listar empleados
            EmpleadoVM empleadoVM = null;
            
            RiakResult<IList<string>> listaKeysEmpleado = client.ListKeysFromIndex(empleadoBucketName);
            foreach (string key in listaKeysEmpleado.Value)
            {
                var result = client.Get(empleadoBucketName, key);
                CheckResult(result);

                empleadoVM = result.Value.GetObject<EmpleadoVM>();
                Console.WriteLine("Empleado :" + empleadoVM.ToJson());
            }


            // 5) Obtener el número de habitación, tipo y precio de las habitaciones que estén ocupadas en la actualidad (no tienen fecha de salida).

            var riakIndexId = new RiakIndexId(facturasBucketName, facturaFacturaSalidaDateIndexName);
            RiakResult<RiakIndexResult> indexRiakResult = client.GetSecondaryIndex(riakIndexId, DateTime.MinValue.ToString("yyyy-MM-dd"));
            CheckResult(indexRiakResult);
            var indexResult = indexRiakResult.Value;
            FacturaVM facturaVMRespuesta = null;
            foreach (var item in indexResult.IndexKeyTerms)
            {
                var result = client.Get(facturasBucketName, item.Key);
                CheckResult(result);

                facturaVMRespuesta = result.Value.GetObject<FacturaVM>();
                Console.WriteLine("FACTURA-- (key values): NumHabitacion: {0}, Tipo: {1}, Precio: {2}, CodigoFactura: {3}",
                    facturaVMRespuesta.Numero.Numero, facturaVMRespuesta.Numero.Tipo.Tipo, facturaVMRespuesta.Numero.Tipo.precio, 
                    facturaVMRespuesta.CodigoF);
            }

            Console.WriteLine("--------------------------");

            //2) Obtener el nombre del jefe del servicio de "Restaurante".

            riakIndexId = new RiakIndexId(empleadoBucketName, empleadoServicioDescripcionIndexName);
            indexRiakResult = client.GetSecondaryIndex(riakIndexId, "restaurante");
            CheckResult(indexRiakResult);
            indexResult = indexRiakResult.Value;
            EmpleadoVM empleadoVMRes = null;
            foreach (var item in indexResult.IndexKeyTerms)
            {
                var result = client.Get(empleadoBucketName, item.Key);
                CheckResult(result);

                empleadoVMRes = result.Value.GetObject<EmpleadoVM>();
                Console.WriteLine("Nobre Jefe Ser Restaurante; {0} ", empleadoVMRes.Servicio.NumReg.Nombre);
            }

            Console.WriteLine("--------------------------");
            //3) Obtener el nombre del jefe de "Jorge Alonso Alonso".

            riakIndexId = new RiakIndexId(empleadoBucketName, empleadoNombreIndexName);
            indexRiakResult = client.GetSecondaryIndex(riakIndexId, "Jorge Alonso Alonso");
            CheckResult(indexRiakResult);
            indexResult = indexRiakResult.Value;

            EmpleadoVM empleadoVMRes1 = null;
            foreach (var item in indexResult.IndexKeyTerms)
            {
                var result = client.Get(empleadoBucketName, item.Key);
                CheckResult(result);
                empleadoVMRes1 = result.Value.GetObject<EmpleadoVM>();
                Console.WriteLine("Nobre Jefe a cargo jorge alonso; {0} ", 
                    empleadoVMRes1.Servicio.NumReg.Nombre);
            }


            Console.WriteLine("--------------------------");

            //4) Obtener un listado de los empleados y los servicios a los que están asignados,
            //exclusivamente para aquellos que tengan algún servicio asignado.

            riakIndexId = new RiakIndexId(empleadoBucketName, empleadoCodSIndexName);
            indexRiakResult = client.GetSecondaryIndex(riakIndexId, 1,1000);
            CheckResult(indexRiakResult);
            indexResult = indexRiakResult.Value;

            EmpleadoVM empleadoVMRes2 = null;
            foreach (var item in indexResult.IndexKeyTerms)
            {
                var result = client.Get(empleadoBucketName, item.Key);
                CheckResult(result);
                empleadoVMRes2 = result.Value.GetObject<EmpleadoVM>();
                Console.WriteLine("Nobre Empleado con Ser; {0} ", empleadoVMRes2.Nombre);
            }



        }
    }
}
