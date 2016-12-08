using System;
using System.Collections.Generic;
using System.Threading;


//
// programa: Barbeiro_Sonolento
// programador: Giorge Caique
// data: 12/10/2016
// descricao: Programa que soluciona o problema do barbeiro sonolento usando threads e monitor
// entrada(s): Pelo sistema
// saida(s): Pela tela, o estado da sala de espera, do barbeiro e do salão de corte
//
namespace Exercicio6._3B_TP_SO
{
    public class ThreadWork
    {
        private static int Id;
        static private string barber;
        static private List<int> waitingRoom = new List<int>();
        static private object lockHairCutRoom = new object();
        static private object lockWaitingRoom = new object();

        public ThreadWork()
        {
            Thread tWaitingRoom = new Thread(() => ThreadWaitingRoom()); //Thread da sala de espera
            Thread tBarber = new Thread(() => ThreadBarber()); //Thread do barbeiro
            tWaitingRoom.Start();
            tBarber.Start();

        }

        public void ThreadWaitingRoom()
        {

            for (int i = 0; i < 15; i++)
            {
                Id = i;
                WaitingRoom();
            }
        }

        public void ThreadBarber()
        {
            while (true)
            {
                try
                {
                    if (waitingRoom.Count == 0)
                    {
                        barber = "dormindo";
                        Printer.BarberSleep();
                        Thread.Sleep(100);
                    }
                    else if (waitingRoom.Count >= 1 && barber == "dormindo")
                    {
                        barber = "acordado";
                        Printer.BarberWakeUp();
                    }
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }
        }


        public void WaitingRoom()
        {
            Thread.Sleep(700);
            if (waitingRoom.Count <= 9) // Lista de clientes na sala de espera deve ser menor que quantidade de cadeiras
            {
                try
                {
                    Monitor.Enter(lockWaitingRoom); // Ativa o lock do monitor   
                    Monitor.Pulse(lockWaitingRoom); //Emite o pulse da mudança do lock
                    waitingRoom.Add(Id); // Adiciona o cliente a lista
                    Printer.WaitingRoom();
                }
                finally
                {
                    Monitor.Exit(lockWaitingRoom); // Libera o lock do monitor
                    Thread tSalaoCorte = new Thread(() => HairCutRoom()); //Thread do salão de corte
                    tSalaoCorte.Start();
                }
            }
            else // Caso não haja cadeiras disponíveis 
            {
                try
                {
                    Printer.WaitingRoomFull();
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }
        }

        public void HairCutRoom()
        {
            try
            {
                Monitor.Enter(lockHairCutRoom); // Ativa o lock do monitor   
                Monitor.Pulse(lockHairCutRoom); //Emite o pulse da mudança do lock
                Printer.LeaveWaitingRoom();
                Printer.HairCutRoom();
            }
            finally
            {
                Thread.Sleep(4000);
                Printer.LeaveHairCutRoom();
                waitingRoom.RemoveAt(0); //Remove cliente da lista da sala de espera
                Monitor.Exit(lockHairCutRoom); //Libera o lock do monitor
            }
        }
    }

    static public class Printer
    {
        static public void BarberSleep()
        {
            Console.WriteLine("Sala vazia! Barbeiro dorme");
        }

        static public void BarberWakeUp()
        {
            Console.WriteLine("Cliente chegou! Barbeiro acorda");
        }
        static public void HairCutRoom()
        {
            Console.WriteLine("Cliente entrou no salão de corte");
        }

        static public void WaitingRoom()
        {
            Console.WriteLine("Cliente entrou na sala de espera ");
        }

        static public void WaitingRoomFull()
        {
            Console.WriteLine("Cliente desistiu! Sala de espera cheia");
        }

        static public void LeaveWaitingRoom()
        {
            Console.WriteLine("Cliente saiu da sala de espera");
        }

        static public void LeaveHairCutRoom()
        {
            Console.WriteLine("Cliente saiu do salão de corte");
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            ThreadWork tw = new ThreadWork();
            Console.ReadKey();
        }
    }
}