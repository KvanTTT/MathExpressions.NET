MathExpressions.NET
==============

A library for parsing math expressions with rational numbers, finding their derivatives and compiling an optimal IL code.

##Libraries##
* [GOLD Parsing System](http://goldparser.org/) - Code generation from math expression grammar.
* [WolframAlpha.NET](https://github.com/Genbox/WolframAlpha.NET) - Symbolic derivatives testing.
* ILSpy - IL assembly dissambler. For compilation tesing.
* NUnit - General testing purposes.

##Using##

* Simplification

  ```csharp
  var func = new MathFunc("(2 * x ^ 2 - 1 + 0 * a) ^ -1 * (2 * x ^ 2  - 1 * 1) ^ -1").Simplify();
  // func == (x ^ 2 * 2 + -1) ^ -2;
  ```

* Differentiation

  ```csharp
  var func = new MathFunc("(2 * x ^ 2 - 1 + 0 * a) ^ -1 * (2 * x ^ 2  - 1 * 1) ^ -1").GetDerivative();
  // func == -((x ^ 2 * 2 + -1) ^ -3 * x * 8)
  ```
 
* Compilation
 
  * Dynamic using
    ```csharp
    using (var mathAssembly = new MathAssembly("(2 * x ^ 2 - 1 + 0 * a) ^ -1 * (2 * x ^ 2  - 1 * 1) ^ -1", "x"))
    {
      var funcResult = mathAssembly.Func(5);
      // funcResult == 0.00041649312786339027 (precision value is -1/2401)
      var funcDerResult = mathAssembly.FuncDerivative(5);
      // funcDerResult == -0.00033999439009256349 (precision value is -40/117649)
    }
    ```
 
  * Static using (more faster and conventional)
 
    You sholud compile assembly with MathExpressions.NET and add reference to this assembly your project.
    For function: (2 * x ^ 2 - 1 + 0 * a) ^ -1 * (2 * x ^ 2  - 1 * 1) ^ -1 with variable of x, you'll get:
    ```csharp
    var funcResult = MathFuncLib.MathFunc.Func(5);
    // funcResult == 0.00041649312786339027 (precision value is -1/2401)
    var funcDerResult = MathFuncLib.MathFunc.FuncDerivative(5);
    // funcDerResult == -0.00033999439009256349 (precision value is -40/117649)
    ```
 
  * Undefined constants and functions
 
    ```csharp
    using (var mathAssembly = new MathAssembly("b(x) + 10 * x * a", "x"))
    {
      var b = new Func<double, double>(x => x * x);
      var funcResult = mathAssembly.Func(5, 2, b); // x = 5; a = 2; b = x ^ 2
      // funcResult == 5 ^ 2 + 10 * 5 * 2 = 125
      var funcDerResult = mathAssembly.FuncDerivative(5, 2, b); // x = 5; a = 2; b = x ^ 2
      // funcDerResult == (b(x + dx) - b(x)) / dx + 10 * a = 30
    }
    ```

## Types of MathNodes ##

* **Calculated** - Calculated *decimal* constant.
* **Value** - Calculated constant of *Rational<long, long>* format. Based on [Stephen M. McKamey implementation](http://exif-utils.googlecode.com/svn/trunk/ExifUtils/ExifUtils/Rational.cs).
* **Constant** - Undefined constant. It have name such as *a*, *b* etc.
* **Variable** - It have name, such as *x*, *y* etc.
* **Function** - This node present known (*sin(x)*, *log(x, 3)*, x + a) or unknown (a(x), b'(x)) function. It may have one or more childs.

## Steps of  math expression processing ##

* **Parsing and AST building**. Implemented with LALR [GOLD Parsing System](http://goldparser.org/). Output of this step is tree structure of MathFuncNode types, which was described above.
  * **Rational Numbers**.
![Rational number representation](http://habr.habrastorage.org/post_images/fc7/c73/526/fc7c73526dc83f8c341aa43f23d2b931.png)
* **Taking of symbolic derivative**. This is the recursive process of replacing simple nodes without childs by constants (0 and 1), taking substitutions from table for known functions (such as for sin(x)' = cos(x)), and replacing unknown functions with themselves with stroke (I mean a(x)' = a'(x)). List of derivatives at first time loaded from [app.config](https://github.com/KvanTTT/Math-Functions/blob/master/MathFunctions.GUI/app.config) but further can be changed in runtime and saved.
  * Calculated' = Value' = Constant' = 0
  * Variable' = 1
  * KnownFunc(x)' = Derivatives\[KnownFunc\](x) * x'
  * UnknownFunc(x)' = UnknownFunc'(x) * x'
* **Simplification**.  This is similar to previous process, but with another substitution rools, such as
  * a * 1 = a
  * a + 0 = a
  * a - a = 0
  * ...

Worth mentioning that commutative functions (addition and multiplication) taken as function with several nodes for more easy and flexible travers.

For properly nodes comparsion, sorting is using, as demonstated on image below:
![Nodes sorting](http://habrastorage.org/files/b78/215/c09/b78215c09a0441b8b96ab5a552da9250.png)

* **Compilation**. At this step simplified tree from previous step transformed to list of IL commands. There are implemented some optimizations:
  * **Fast exponentiation (by squaring)**
  At this step expression with powers converts to optimized form with [exponentiation by squaring algorithm](http://en.wikipedia.org/wiki/Exponentiation_by_squaring). For example: a*a*a*a*a*a will be converted to (a^2)^2 * a^2.
  * **Using result of previously calculated nodes**
If result of calculated value of any function is using more than one time, it can be stored to the local variable and it can be used at further code by such way:
  ```csharp
  if (!func.Calculated)
  {
      EmitFunc(funcNode);
      func.Calculated = true;
  }
  else
      IlInstructions.Add(new OpCodeArg(OpCodes.Ldloc, funcNode.Number));
  ```
  * **Waste IL instruction removing**
For generated IL code for math functions without loops, following optimizations are available:
  ![IL Optimizations](http://habrastorage.org/files/f2b/cc4/866/f2bcc48663a94d0c84c867583aefffc3.png)
  * **Local vars count reducing**
   One local variable is used for every calculated function. But it can be also used for another calculated result. So, it is posiible to reduce number of local variables by such way:
  ![Local vars count reducing](http://habrastorage.org/files/ab0/a27/c29/ab0a27c29a3843af9e32b867b78cf4de.png)

## Testing ##

* **WolframAlpha.NET**
  This lib for comparsion of expected derivative from WolframAlpha API and actual derivative.
* **Assembly loading and unloading**
  .NET assembly has been generated on compilation step. For dynamical assembly loading and unloading ```AppDomain``` is used with ```CreateInstanceFromAndUnwrap```.
* **Comparsion output of csc.exe compiler in release mode and my output.**

I compared generated IL code for example following function:

```x ^ 3 + sin(3 * ln(x * 1)) + x ^ ln(2 * sin(3 * ln(x))) - 2 * x ^ 3```

  **csc.exe .NET 4.5.1**                                    | **MathExpressions.NET**
  ----------------------------------------------------------|--------------------------------------------------
  ldarg.0<br/>ldc.r8 3<br/>call float64 Math::Pow(float64, float64)<br/>ldc.r8 3<br/>ldarg.0<br/>ldc.r8 1<br/>mul<br/>call float64 Math::Log(float64)<br/>mul<br/>call float64 Math::Sin(float64)<br/>add<br/>ldarg.0<br/>ldc.r8 2<br/>ldc.r8 3<br/>ldarg.0<br/>call float64Math::Log(float64)<br/>mul<br/>call float64 Math::Sin(float64)<br/>mul<br/>call float64 Math::Log(float64)<br/>ldc.r8 2<br/>ldarg.0<br/>ldc.r83<br/>call float64 Math::Pow(float64, float64)<br/>mul<br/>sub<br/>call float64 Math::Pow(float64, float64)<br/>add<br/>ret  |  ldarg.0<br/>ldc.r8 2<br/>ldc.r8 3<br/>ldarg.0<br/>call float64 Math::Log(float64)<br/>mul<br/>call float64 Math::Sin(float64)<br/>stloc.0<br/>ldloc.0<br/>mul<br/>call float64 Math::Log(float64)<br/>call float64 Math::Pow(float64,float64)<br/>ldarg.0<br/>ldarg.0<br/>mul<br/>ldarg.0<br/>mul<br/>sub<br/>ldloc.0<br/>add<br/>ret<br/><br/><br/><br/><br/><br/><br/><br/><br/>


More detail explanation available [on Russian](http://habrahabr.ru/post/150043/)
