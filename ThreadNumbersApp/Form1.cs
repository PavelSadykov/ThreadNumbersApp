using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;


namespace ThreadNumbersApp
{
    public partial class Form1 : Form
    {
      
        Thread primeThread;// поток для простых чисел
        Thread fbThread;// поток для  чисел Фибоначчи
        private CancellationTokenSource tokenSource1;//флаг для остановки потока простых чисел
        private CancellationTokenSource tokenSource2;//флаг для остановки потока чисел Фибоначчи
        private bool isGenerating;//лог переменная процесса генерации потока простых чисел
        private bool isGeneratingFibonacci;//лог переменная процесса генерации потока  чисел Фибоначчи
        List<int>primeNumbers = new List<int>();// список для простых чисел
        List<int> fbNumbers = new List<int>();// список для чисел Фб
        public Form1()
        {

            InitializeComponent();
            tokenSource1 = new CancellationTokenSource();
            tokenSource2 = new CancellationTokenSource();   
        }

       //*********ПРОСТЫЕ ЧИСЛА **************
        private void button1_Click(object sender, EventArgs e)
        {
            tokenSource1 = new CancellationTokenSource();
            if (!isGenerating)
            {
                // если процесс не запущен, метод получает нач и конечное значения и начинает генерацию в потоке
                // если данные не введены , диапазон по умолчанию от 2 до 500
                int startNumber = string.IsNullOrEmpty(txtStart.Text) ? 2 : int.Parse(txtStart.Text);
                int endNumber = string.IsNullOrEmpty(txtEnd.Text) ? 500 : int.Parse(txtEnd.Text);

                isGenerating = true;


                 GeneratePrimeNumbers(startNumber, endNumber);//отправляем в обработку для получения списка простых чисел

                primeThread = new Thread(() => GenerateAndShowNumbers(tokenSource1, listBox1, primeNumbers));
                primeThread.Start();
            }
            
        }

        //метод генерации простых чисел
        private void GeneratePrimeNumbers(int start, int end)
        {
            for (int i = start; i <= end && isGenerating; i++)
            {
                if (IsPrime(i))
                {
                    primeNumbers.Add(i);//добавляем в список простые числа
                }
            }
            isGenerating = false;
        }


        //метод проверки числа на простое 
        private bool IsPrime(int number)
        {
            if (number <= 1)
                return false;

            for (int i = 2; i <= Math.Sqrt(number); i++)// метод проверки деления на все числа от 2 до квадр корня из числа
            {
                if (number % i == 0)
                    return false;
            }

            return true;
        }


        //*************ЧИСЛА ФИБОНАЧЧИ*********
        private void button2_Click_1(object sender, EventArgs e)
        {
            tokenSource2 = new CancellationTokenSource();
            if (!isGeneratingFibonacci)
            {
                // если процесс не запущен, метод получает нач и конечное значения и начинает генерацию в потоке
                int startNumber = string.IsNullOrEmpty(txtStart.Text) ? 2 : int.Parse(txtStart.Text);
                int endNumber = string.IsNullOrEmpty(txtEnd.Text) ? 1000000 : int.Parse(txtEnd.Text);

                isGeneratingFibonacci = true;


                GenerateFibonacciInRange(startNumber, endNumber);//отправляем в обработку для получения списка чисел ФБ


                fbThread = new Thread(() => GenerateAndShowNumbers(tokenSource2, listBox2, fbNumbers));
                fbThread.Start();
            }
           
        }

        //метод генерации списка  чисел Фибоначчи

        private void GenerateFibonacciInRange(int start, int end)
        {
            List<int> fibonacciInRange = GetFibonacciInRange(start, end);// сохранение списка в переменную 

            this.Invoke((MethodInvoker)delegate //Инвок для безопасного обновления из другого потока
            {
                foreach (int number in fibonacciInRange)// перебираем
                {
                  fbNumbers.Add(number);//выводим числа Фиб. из списка  в лист бокс
                   
                }
            });

            isGeneratingFibonacci = false;// флаг завершения процесса генерации чисел
          
        }

        private List<int> GetFibonacciInRange(int start, int end)
        {
            List<int> fibonacciInRange = new List<int>();//инициализируем пустой список для хранения чисел Фиб.

            int a = 0, b = 1;//начальные значения
            int temp = 0;

            while (temp <= end)
            {
                temp = a + b;// вычисление ччисел Фб.
                a = b;
                b = temp;

                if (temp >= start && temp <= end)
                {
                    fibonacciInRange.Add(temp);// добавляем  в список
                }
            }

            return fibonacciInRange;// возвращаем список с числами в диапазоне
        }

        //выводим на форму поток
        private void GenerateAndShowNumbers(CancellationTokenSource tokenSource,   ListBox listBox, List<int>numbers)
        {
            try
            {
                foreach (int number in numbers)
                {
                    if (tokenSource.IsCancellationRequested)//если поток остановлен
                        break;

                    listBox.BeginInvoke((MethodInvoker)delegate {
                        listBox.Items.Add(number);//вывод на форму 
                    });

                    Thread.Sleep(200);//задержка для вывода на форму (для удобства восприятия)
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Restart");
            }
           
           

        }
      




        private void button3_Click(object sender, EventArgs e)
        {
           tokenSource1.Cancel();//поток остановлен
        }
       
        private void button4_Click(object sender, EventArgs e)
        {
            
            tokenSource2.Cancel();//поток остановлен
        }
        [Obsolete]
        private void buttonIdle_Click(object sender, EventArgs e)
        {
            if(primeThread!=null && primeThread.IsAlive)
            {//поток выполняется
                primeThread.Suspend();//приостанавливаем
            }
            //поток не выполняется ( завершен или не был создан)
        }
        [Obsolete]
        private void buttonContinue_Click(object sender, EventArgs e)
        {
            if(primeThread != null && primeThread.ThreadState == ThreadState.Suspended)
            {//поток выполняется и его статус - приостановлен
                primeThread.Resume();//возобновляем
            }
        }
        [Obsolete]
        private void buttonIdle2_Click(object sender, EventArgs e)
        {
            if (fbThread != null && fbThread.IsAlive)
            {
                fbThread.Suspend();
            }
        }
        [Obsolete]
        private void buttonContinue2_Click(object sender, EventArgs e)
        {
            if (fbThread != null && fbThread.ThreadState == ThreadState.Suspended)
            {
                fbThread.Resume();
            }
        }

        private void buttonRestartPrime_Click(object sender, EventArgs e)
        {
            tokenSource1.Cancel();//поток остановлен
            tokenSource2.Cancel();//поток остановлен
            isGenerating = false;//флаги в исходное состояние
            isGeneratingFibonacci = false;
            listBox1.Items.Clear();//очищаем элементы на форме от значений
            listBox2.Items.Clear();
            txtEnd.Clear();
            txtStart.Clear();
            primeNumbers.Clear();//очищаем списки от значений
            fbNumbers.Clear();
        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


    }
}
