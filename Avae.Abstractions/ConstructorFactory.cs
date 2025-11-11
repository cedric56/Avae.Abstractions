using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Avae.Abstractions
{
    //A generic factory.
    public static class ConstructorFactory<T>
    {

        // ------------------------------------------------
        // Internal class to make builder lambda functions
        // ------------------------------------------------
        private static class BuilderFactoryImpl<BUILD_FUNCTION>
        {

            // Given a derived type, and some arbitrary parameters
            // (defined in BUILD_FUNCTION), return a constructor.
            public static BUILD_FUNCTION
            buildBuilderWithArgs(Type derivedType = null)
            {

                // If no derived type specified, use type of T.
                // --------------------------------------------
                derivedType = derivedType ?? typeof(T);

                // Check to make sure derivedType is T.
                // ------------------------------------
                if (!derivedType.IsSubclassOf(typeof(T))
                   && derivedType != typeof(T))
                {
                    throw new ArgumentException("Type error.");
                }

                // Get the generic arguments from the functor.
                // -------------------------------------------
                var genericArguments =
                    typeof(BUILD_FUNCTION).GetGenericArguments();

                // One of the arguments is the return value.
                // We are interested in the parameters types.
                // ------------------------------------------
                var numArgs = genericArguments.Count() - 1;

                // Get the parameters and forget about the return type.
                // ----------------------------------------------------
                var buildFunctionParams = genericArguments.Take(numArgs);

                // Find the constructor with the same parameters.
                // ----------------------------------------------
                var ctor = derivedType
                     .GetConstructor(buildFunctionParams.ToArray());

                // Using LINQ, create Expression parameters with the names 
                // and types matching the constructor.
                // -------------------------------------------------------
                var parameters = ctor
                     .GetParameters()
                     .Select(p => Expression
                        .Parameter(p.ParameterType, p.Name))
                     .ToArray();

                // A lambda expression that creates a new instance 
                // of the type of derivedType using the constructor
                // requested.  It's compiled for performance.
                // ------------------------------------------------    
                var e = Expression.New(ctor, parameters);

                // Compile a lambda function that performs new.
                // -------------------------------------------- 
                return Expression
                     .Lambda<BUILD_FUNCTION>(e, parameters)
                     .Compile();
            }
        }
        // ---------------------------------------
        // Helper functions to create the lambdas.
        // ---------------------------------------
        public static Func<T>
        CreateBuilder(Type derivedType = null)
        {
            return BuilderFactoryImpl<Func<T>>
                      .buildBuilderWithArgs(derivedType);
        }
        public static Func<P1, T>
        CreateBuilderWithArgs<P1>(Type derivedType = null)
        {
            return BuilderFactoryImpl<Func<P1, T>>
                      .buildBuilderWithArgs(derivedType);
        }
        public static Func<P1, P2, T>
        CreateBuilderWithArgs<P1, P2>(Type derivedType = null)
        {
            return BuilderFactoryImpl<Func<P1, P2, T>>
                      .buildBuilderWithArgs(derivedType);
        }

        public static Func<P1, P2, P3, T>
        CreateBuilderWithArgs<P1, P2, P3>(Type derivedType = null)
        {
            return BuilderFactoryImpl<Func<P1, P2, P3, T>>
                      .buildBuilderWithArgs(derivedType);
        }

        public static Func<P1, P2, P3, P4, T>
        CreateBuilderWithArgs<P1, P2, P3, P4>(Type derivedType = null)
        {
            return BuilderFactoryImpl<Func<P1, P2, P3, P4, T>>
                      .buildBuilderWithArgs(derivedType);
        }

        // You see the pattern here? You can add more parameters as you like...
    }
}
