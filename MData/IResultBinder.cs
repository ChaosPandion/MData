using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MData
{
    public interface IBinder
    {
        IBinder<T> BindOne<T>() where T : new();
        IBinder<T> BindOne<T>(Func<T> createInstance);

        IBinder<T> BindMany<T>() 
            where T : new();
    }

    public interface IBinder<T>
    {
        IBinder<T> BindRecord();
        IBinder<T> BindRecord(Func<IFieldBinder<T>, IFieldBinder<T>> fieldBinder);

        IBinder<T> BindChild<U>(Expression<Func<T, U>> field, Func<IBinder, IBinder<U>> childBinder);
        IBinder<T> BindChildCollection<U>(Expression<Func<T, Action<U>>> adder, Func<IBinder, IBinder<U>> childBinder);
    }

    public interface IFieldBinder<T>
    {
        IFieldBinder<T> Bind<U>(Expression<Func<T, U>> expression);
    }


    //public interface IResultBinder
    //{
    //    IResultBinder BindEntity<T>(Func<T> createInstance, Func<IEntityResultBinder<T>, IEntityResultBinder<T>> binder);
    //    IResultBinder BindEntityCollection<T>(Func<T> createInstance, Func<IEntityCollectionResultBinder<T>, IEntityCollectionResultBinder<T>> binder);
    //}

    //public interface IEntityResultBinder<T>
    //{
    //    IEntityResultBinder<T> BindRecord();
    //    IEntityResultBinder<T> BindField<U>(Expression<Func<T, U>> expression);
    //    IEntityResultBinder<T> BindChildEntity<U>(Func<U> createInstance, Func<IEntityResultBinder<U>, IEntityResultBinder<U>> binder);
    //    IEntityResultBinder<T> BindChildEntityCollection<U>(Func<U> createInstance, Func<IEntityCollectionResultBinder<U>, IEntityCollectionResultBinder<U>> binder);
    //}

    //public interface IEntityCollectionResultBinder<T>
    //{
    //    IResultBinder BindRecord<T>(Func<IRecordBinder, IRecordBinder> binder);
    //    IResultBinder BindRecords<T>();
    //}

    //public interface IRecordBinder
    //{

    //}
}
