using System;

namespace DesignPatternChallenge
{
    // Enumeração para definir os tamanhos disponíveis.
    // O custo dos complementos (decoradores) dependerá deste valor.
    public enum CoffeeSize { Small, Medium, Large }

    // --- CLASSE BASE (Component) ---
    // Define a interface comum para o café puro e para os decoradores.
    // Isso permite que o sistema trate um "Café com Leite e Mel" apenas como um "Beverage".
    public abstract class Beverage
    {
        public CoffeeSize Size { get; set; } = CoffeeSize.Small;
        public abstract string GetDescription();
        public abstract decimal GetCost();
    }

    // --- CLASSES CONCRETAS (Concrete Components) ---
    // Representam o núcleo do pedido. Elas não conhecem os complementos.
    public class Espresso : Beverage
    {
        public override string GetDescription() => $"Espresso {Size}";

        public override decimal GetCost()
        {
            // O preço base muda de acordo com o tamanho escolhido.
            return Size switch
            {
                CoffeeSize.Small => 3.50m,
                CoffeeSize.Medium => 4.50m,
                CoffeeSize.Large => 5.50m,
                _ => 3.50m
            };
        }
    }

    // --- O DECORADOR BASE (Decorator) ---
    // Esta é a classe "mágica". Ela herda de Beverage (para ter o mesmo tipo)
    // mas também CONTÉM uma Beverage (composição). 
    public abstract class CondimentDecorator : Beverage
    {
        protected Beverage _beverage; // O objeto que está sendo "embrulhado"

        protected CondimentDecorator(Beverage beverage)
        {
            _beverage = beverage;
            // IMPORTANTE: O decorador herda o tamanho da bebida que ele envolve.
            // Isso garante que se o café for Grande, o Leite saiba que deve cobrar o preço de Grande.
            this.Size = beverage.Size;
        }
    }

    // --- DECORADORES CONCRETOS ---
    
    // 1. Decorador de Leite
    public class Milk : CondimentDecorator
    {
        public Milk(Beverage beverage) : base(beverage) { }

        // Chamada Recursiva: Ele pega a descrição do que está dentro e adiciona a dele.
        public override string GetDescription() => _beverage.GetDescription() + ", Milk";

        public override decimal GetCost()
        {
            // Lógica de Preço Dinâmica: 
            // O custo do leite é calculado somando o custo da bebida interna + o adicional por tamanho.
            decimal milkCost = Size switch
            {
                CoffeeSize.Small => 0.30m,
                CoffeeSize.Medium => 0.50m,
                CoffeeSize.Large => 0.70m,
                _ => 0.50m
            };

            return _beverage.GetCost() + milkCost;
        }
    }

    // 2. Decorador de Chocolate
    public class Chocolate : CondimentDecorator
    {
        public Chocolate(Beverage beverage) : base(beverage) { }

        public override string GetDescription() => _beverage.GetDescription() + ", Chocolate";

        public override decimal GetCost()
        {
            // O Chocolate adiciona um valor fixo de 0.70m independente do tamanho
            // (mostrando que cada decorador tem total controle sobre sua regra de negócio).
            return _beverage.GetCost() + 0.70m;
        }
    }

    // --- EXECUÇÃO ---
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== SISTEMA DE PEDIDOS (DECORATOR PATTERN) ===\n");

            // Exemplo 1: Espresso Pequeno Simples
            Beverage simpleEspresso = new Espresso { Size = CoffeeSize.Small };
            PrintOrder(simpleEspresso);

            // Exemplo 2: Espresso Grande com DUPLO Chocolate e Leite
            // Aqui vemos o poder do padrão: "Envolvemos" o objeto várias vezes.
            Beverage fancyCoffee = new Espresso { Size = CoffeeSize.Large };
            fancyCoffee = new Chocolate(fancyCoffee); // Camada 1: Chocolate
            fancyCoffee = new Chocolate(fancyCoffee); // Camada 2: Mais Chocolate!
            fancyCoffee = new Milk(fancyCoffee);      // Camada 3: Leite (que cobrará preço de Large)
            
            PrintOrder(fancyCoffee);
        }

        static void PrintOrder(Beverage beverage)
        {
            // O cliente do código não precisa saber quantos decoradores existem.
            // Ele apenas chama GetDescription() e o objeto resolve a recursão sozinho.
            Console.WriteLine($"PEDIDO: {beverage.GetDescription()}");
            Console.WriteLine($"TOTAL : R$ {beverage.GetCost():N2}");
            Console.WriteLine(new string('-', 40));
        }
    }
}