using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;
using System.Configuration;
using System.Web.Mvc;
using VideoSystem.Abstract;
using VideoSystem.Concrete;

namespace VideoSystem.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyResolver()
        {
            kernel = new StandardKernel();
            AddBindings();
        }

        public Object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<Object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        public void AddBindings()
        {
            //将接口绑定到它的实现
            kernel.Bind<IVerifyCode>().To<SimpleVerifyCode>();
            kernel.Bind<IPaging>().To<Paging>();
            kernel.Bind<IUploadFile>().To<UploadFile>();
            kernel.Bind<ICreateCode>().To<CreateCode>();
            kernel.Bind<IEncryption>().To<Encryption>();
            kernel.Bind<IExportExcel>().To<ExportExcel>();
        }
    }
}