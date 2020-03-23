using System;

namespace UpgradeHelpers.Helpers
{
    /// <summary>
    /// The ErrorHandlingHelper provides util functionality for Error Handling support. Specially the VB6
    /// Resume Next handling.
    /// </summary>
    public class ErrorHandlingHelper
    {
 
        /// <summary>
        /// Delegate for each Resume Next stat sent to the ResumeNext method.
        /// </summary>
        public delegate void ResumeNextStatDelegate();
        /// <summary>
        /// Delegate for each Resume Next expression sent to the ResumeNextExpr method.
        /// </summary>
        public delegate object ResumeNextExprDelegate();

        /// <summary>
        /// Simulates the VB6 ResumeNext statement executing each statement inside a try/catch block,
        /// avoiding the throwing of exceptions.
        /// </summary>
        /// <param name="stats">A list of ResumeNextDelegate containing a list of statements to be executed in a try/catch block.</param>
        public static void ResumeNext(params ResumeNextStatDelegate[] stats)
        {
            Exception ex;
            ResumeNext(out ex, stats);
        }

        /// <summary>
        /// Simulates the VB6 ResumeNext statement executing each statement inside a try/catch block,
        /// avoiding the throwing of exceptions.
        /// </summary>
        /// <param name="ex">An Exception if any is thrown executing the statements or null if not.</param>
        /// <param name="stats">A list of ResumeNextDelegate containing a list of statements to be executed in a try/catch block.</param>
        public static void ResumeNext(out Exception ex, params ResumeNextStatDelegate[] stats)
        {
            ex = null;
            foreach (ResumeNextStatDelegate stat in stats)
            {
                try
                {
                    stat.Invoke();
                }
                catch (Exception e)
                {
                    ex = e;
                }
            }
        }

        /// <summary>
        /// Executes an expression inside a try/catch block to avoid throwing of exception. This expression is being
        /// executed inside a ResumeNext statement, for instance a condition of if, while, for, etc.
        /// </summary>
        /// <typeparam name="T">The type of the class being created</typeparam>
        /// <param name="expr">The expression to be executed in a try/catch block.</param>
        public static T ResumeNextExpr<T>(ResumeNextExprDelegate expr)
        {
            Exception ex;
            return ResumeNextExpr<T>(out ex, expr);
        }

        /// <summary>
        /// Executes an expression inside a try/catch block to avoid throwing of exception. This expression is being
        /// executed inside a ResumeNext statement, for instance a condition of if, while, for, etc.
        /// </summary>
        /// <typeparam name="T">The type of the class being created</typeparam>
        /// <param name="ex">An Exception if any is thrown executing the expression or null if not.</param>
        /// <param name="expr">The expression to be executed in a try/catch block.</param>
        public static T ResumeNextExpr<T>(out Exception ex, ResumeNextExprDelegate expr)
        {
            T result = default(T);
            ex = null;
            try
            {
                result = (T)expr.Invoke();
            }
            catch (Exception e)
            {
                ex = e;
            }
            return result;
        }
    }
}
