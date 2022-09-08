using System;
using System.Collections;
using System.Linq.Expressions;

namespace rhitmo
{

    class Program
    {
        static void Main(string[] args)
        {
            var estacionamento = new Estacionamento(1,1, 3);
            Console.WriteLine($"Vazio ? {estacionamento.Vazio()}");

            IVeiculo
            a = new Moto() { Placa = "a" },
            b = new Carro() { Placa = "b" },
            c = new Van() { Placa = "c" },
            d = new Moto() { Placa = "d" },
            e = new Van() { Placa = "e" };

            estacionamento.Estacionar(a);

            estacionamento.Estacionar(b);

            estacionamento.Estacionar(c);

            estacionamento.Estacionar(e);

            estacionamento.Estacionar(d);


            void PrintAll()
            {

                Console.WriteLine("\n-----------------------------");

                Console.WriteLine($"Lista de Lotados: {string.Join(" | ", estacionamento.ListaLotados())}");

                Console.WriteLine($"Estacionamento está cheio ? {estacionamento.Cheio()}");
                Console.WriteLine($"Quantas vagas as vans estão ocupadas ? {estacionamento.QtdVagasVans()}");
                Console.WriteLine($"Quantas vagas as motos estão ocupadas ? {estacionamento.QtdVagasMoto()}");
                Console.WriteLine($"Quantas vagas os carros estão ocupadas ? {estacionamento.QtdVagasCarro()}");
                Console.WriteLine($"Restante de vagas ? {estacionamento.RestanteVagas()}");
                Console.WriteLine($"Total de vagas ? {estacionamento.TotalVagas()}");
            }

            PrintAll();


            estacionamento.Sair(a);
            estacionamento.Sair(b);
            estacionamento.Sair(c);
            estacionamento.Sair(d);
            estacionamento.Sair(e);

            PrintAll();

        }
    }





    public class Estacionamento
    {
        List<Vaga> Vagas { get; set; } = new();

        int _QtdVagasCarro { get; set; }
        int _QtdVagasMotos { get; set; }
        int _QtdVagasGrandes { get; set; }

        int total;

        public Estacionamento(int QtdVagasGrandes, int qtdVagasMotos, int QtdVagasCarro)
        {
            this._QtdVagasCarro = QtdVagasCarro;
            this._QtdVagasMotos = qtdVagasMotos;
            this._QtdVagasGrandes = QtdVagasGrandes;

            this.total = qtdVagasMotos + QtdVagasCarro + QtdVagasGrandes;
        }

        public void Estacionar(IVeiculo veiculo)
        {
            var valida = this.TemVaga(new Vaga(veiculo));

            if (!valida.preenchida)
            {
                Console.WriteLine("Acabaram as vagas!");
                return;
            }

            this.Vagas.Add(valida.vaga);
        }

        public void Sair(IVeiculo veiculo)
        {
            var vaga = this.Vagas.FirstOrDefault(o => o.Veiculo.Placa.Equals(veiculo.Placa,
                StringComparison.OrdinalIgnoreCase))!;

            if (vaga is null) return;

            this.DevolveVaga(vaga);

            this.Vagas.Remove(vaga);

        }

        void DevolveVaga(Vaga vaga)
        {
            #region [Devolvendo vaga de carro]

            if (vaga.TipoVaga == Vaga.ETipoVaga.Carro && vaga.Veiculo is not Van)
                this._QtdVagasCarro++;
            else if (vaga.TipoVaga == Vaga.ETipoVaga.Carro)
                this._QtdVagasCarro += 3;
            #endregion

            #region [Devolvendo vaga de moto]
            if (vaga.TipoVaga == Vaga.ETipoVaga.Moto)
                this._QtdVagasMotos++;
            #endregion


            #region [Devolvendo vaga grande]
            if (vaga.TipoVaga == Vaga.ETipoVaga.Grande)
                this._QtdVagasGrandes++;

            #endregion
        }


        (bool preenchida, Vaga vaga) TemVaga(Vaga vaga)
        {

            (bool preenchida, Vaga vaga)
              @Return((bool TemVaga, Vaga.ETipoVaga TipoVaga) result)
            {
                vaga.TipoVaga = result.TipoVaga;
                return (result.TemVaga, vaga);
            }

            switch (vaga.Veiculo)
            {
                case Moto:
                    return @Return(ValidaMoto());

                case Carro:
                    return @Return(ValidaCarro());
                case Van:
                    return @Return(ValidaVan());

                default:
                    return default;
            }


        }

        (bool, Vaga.ETipoVaga) ValidaVan()
        {
            if (_QtdVagasGrandes > 0 || _QtdVagasCarro >= 3)
            {
                if (_QtdVagasGrandes > 0)
                {
                    _QtdVagasGrandes--;
                    return (true, Vaga.ETipoVaga.Grande);
                }
                _QtdVagasCarro -= 3;
                return (true, Vaga.ETipoVaga.Carro);
            }

            return (false, default);
        }


        (bool, Vaga.ETipoVaga) ValidaCarro()
        {
            if (_QtdVagasCarro > 0 || _QtdVagasGrandes > 0)
            {
                if (_QtdVagasCarro > 0)
                {
                    _QtdVagasCarro--;
                    return (true, Vaga.ETipoVaga.Carro);
                }

                _QtdVagasGrandes--;
                return (true, Vaga.ETipoVaga.Grande);
            }

            return (false, default);
        }

        (bool, Vaga.ETipoVaga) ValidaMoto()
        {
            if (_QtdVagasMotos > 0)
            {
                _QtdVagasMotos--;
                return (true, Vaga.ETipoVaga.Moto);
            }
            else if (_QtdVagasCarro > 0 || _QtdVagasGrandes > 0)
            {
                if (_QtdVagasCarro > 0)
                {
                    _QtdVagasCarro--;
                    return (true, Vaga.ETipoVaga.Carro);
                }

                _QtdVagasGrandes--;
                return (true, Vaga.ETipoVaga.Grande);
            }
            else
                return (false, default);
        }

        //Diga-nos quantas vagas as vans estão ocupadas
        public int QtdVagasVans() =>
             this.Vagas.Where(o => o.Veiculo is Van)
             .Sum(o =>
             {
                 if (o.TipoVaga == Vaga.ETipoVaga.Carro)
                     return 3;

                 return 1;
             });

        public int QtdVagasCarro() =>
              this.Vagas.Where(o => o.Veiculo is Carro)
                .Count();

        public int QtdVagasMoto() =>
              this.Vagas.Where(o => o.Veiculo is Moto)
                .Count();

        //Diga-nos quando o estacionamento estiver vazio
        public bool Vazio() => !this.Vagas.Any();

        //Diga-nos quando certos lugares estão cheios, por exemplo quando todas as vagas de moto são ocupadas
        public List<Vaga.ETipoVaga> ListaLotados()
        {
            var tipoVagaLotados = new List<Vaga.ETipoVaga>();

            if (_QtdVagasCarro == 0 && this.Vagas.Any(o => o.TipoVaga == Vaga.ETipoVaga.Carro))
                tipoVagaLotados.Add(Vaga.ETipoVaga.Carro);

            if (_QtdVagasGrandes == 0 && this.Vagas.Any(o => o.TipoVaga == Vaga.ETipoVaga.Grande))
                tipoVagaLotados.Add(Vaga.ETipoVaga.Grande);

            if (_QtdVagasMotos == 0 && this.Vagas.Any(o => o.TipoVaga == Vaga.ETipoVaga.Moto))
                tipoVagaLotados.Add(Vaga.ETipoVaga.Moto);

            return tipoVagaLotados;

        }

        //Diga-nos quando o estacionamento estiver cheio
        public bool Cheio() => _QtdVagasCarro == 0 && _QtdVagasGrandes == 0 && _QtdVagasMotos == 0 && this.Vagas.Any();

        //Diga - nos quantas vagas restam
        public int RestanteVagas() => _QtdVagasMotos + _QtdVagasCarro + _QtdVagasGrandes;

        //Diga-nos quantas vagas totais há no estacionamento    
        public int TotalVagas() => total;
    }

    public class Vaga
    {
        public ETipoVaga TipoVaga { get; set; }

        public IVeiculo Veiculo { get; set; }

        public Vaga(IVeiculo veiculo)
        {
            this.Veiculo = veiculo;
        }

        public enum ETipoVaga
        {
            Carro,
            Moto,
            Grande
        }
    }

    public class Carro : IVeiculo
    {
        public decimal KmPorHora { get; set; }
        public decimal VolumePortaMala { get; set; }
        public int QuantidadeLugares { get; set; }
        public int QuantidadePortas { get; set; }
        public string Placa { get; set; }
    }
    public class Moto : IVeiculo
    {
        public decimal KmPorHora { get; set; }
        public string Placa { get; set; }
    }
    public class Van : IVeiculo
    {
        public decimal KmPorHora { get; set; }
        public int QuantidadeLugares { get; set; }
        public string Placa { get; set; }

    }

    public interface IVeiculo
    {
        decimal KmPorHora { get; set; }
        string Placa { get; set; }

    }


}



