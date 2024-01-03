using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Collections.Generic;

namespace lab4
{
    public partial class Form1 : Form
    {
        private static ulong[] simple_numbers = {2,3,5,7,11,13,17,19,23,29,31,37,41,43,47,
            53,59,61,67,71,73,79,83,89,97,101,103,107,109,113,127,131,137,139,149,
            151,157,163,167,173,179,181,191,193,197,199,211,223,227,229,233,239,
            241,251,257,263,269,271,277,281,283,293,307,311,313,317,331,337,347,
            349,353,359,367,373,379,383,389,397,401,409,419,421,431,433,439,443,449,457,461,463,467,479,487,491,499 };
        private static ulong n = 0; // случайное открытое простое большое число
        private static ulong xa = 0; // секретное число Алисы
        private static ulong xb = 0; // секретное число Боба
        private static ulong ya = 0; // открытое значение Алисы
        private static ulong yb = 0; // открытое значение Боба

        // Алгоритм быстрого возведения в степень
        private static ulong Power(BigInteger x, BigInteger y, ulong mod)
        {
            BigInteger count = 1;
            if (y == 0) return 1;
            while (y > 0)
            {
                if (y % 2 == 0)
                {
                    y /= 2;
                    x *= x;
                    x %= mod;
                }
                else
                {
                    y--;
                    count *= x;
                    count %= mod;
                }
            }
            return (ulong) (count % mod);
        }
        
        public Form1()
        {
            InitializeComponent();
            radioButton1.Checked = true; // выбор 10-ой системы 
            label4.Text = ""; // потраченное время
            radioButton5.Checked = true; // генерация n
            textBox2.Enabled = false; // тестовое поле ввода n
            button3.Enabled = false; // кнопка подтверждения ввода n
            textBox2.Text = ""; // очистка поля ввода n
            label6.Text = ""; // очистка значения n 
            label7.Text = ""; // очистка значения Xa
            radioButton6.Checked = true; // генерация Xa
            textBox3.Enabled = false; // тестовое поле ввода Xa
            textBox3.Text = "";
            button4.Enabled = false; // кнопка подтверждения ввода Xa
            label9.Text = ""; // очистка значения Xb
            radioButton8.Checked = true; // генерация Xb
            textBox4.Enabled = false; // тестовое поле ввода Xb
            textBox4.Text = "";
            button6.Enabled = false; // кнопка подтверждения ввода Xa
            groupBox4.Enabled = false; // схема обмена ключами
            ClearLabels(); // очиста Label в схеме обмена ключами
        }

        // Проверка числа на простоту
        private bool CheckSimple(double num)
        {
            double temp = Math.Sqrt(num);

            double i = 2;

            while (i <= temp)
            {
                if (num % i == 0)
                {
                    return false;
                }
                i++;
            }
            return true;
        }

        //обычный алгоритм Евклида через остатки
        ulong Nod(ulong a, ulong b)
        {
            while ((a > 0) && (b > 0))
                if (a >= b)
                    a %= b;
                else
                    b %= a;
            return a | b;
        }

        // Вычисление первообразного корня (1ое задание) (само вычисление корней)
        private ulong FindPrimitiveRoot(ulong num, bool df)
        {
            if(df) // если корень для алгоритма ДФ, то достаточно найти наименьший
            {
                // Факторизация fi(num)
                List<ulong> factorization = new List<ulong>();
                ulong b, c;
                ulong fiMasCount = num - 1; // т.к. num 100% простое, что функция эйлера на 1 меньше num
                ulong temp = fiMasCount;

                while ((temp % 2) == 0)
                {
                    temp = temp / 2;
                    if (!factorization.Contains(2))
                        factorization.Add(2);
                }
                b = 3;
                c = (ulong)Math.Sqrt(temp) + 1;
                while (b < c)
                {
                    if ((temp % b) == 0)
                    {
                        if (temp / b * b - temp == 0)
                        {
                            if (!factorization.Contains(b))
                                factorization.Add(b);
                            temp = temp / b;
                            c = (ulong)Math.Sqrt(temp) + 1;
                        }
                        else
                            b += 2;
                    }
                    else
                        b += 2;
                }
                if (!factorization.Contains(temp))
                    factorization.Add(temp);
                // Проверка каждоого основания [2...num-1]
                for (ulong i = 2; i < num; i++)
                {
                    bool check = true;
                    foreach (ulong a in factorization)
                    {
                        if (Power(i, fiMasCount / a, num) % num == 1)
                        {
                            check = false;
                            break;
                        }
                    }
                    if (check)
                        return i;
                }
                return 0;
            }
            else // иначе ищем и выводм все корни
            {
                DateTime StartTime = DateTime.Now; // время начало поиска корней
                List<ulong> fiMas = new List<ulong>(); // числа в функции эйлера
                ulong checkingNumber = 1;

                while (checkingNumber < num) // проверяем каждое на взаимную простоту с num
                {
                    if (Nod(checkingNumber, num) == 1)
                    {
                        fiMas.Add(checkingNumber);
                    }
                    checkingNumber++;
                }
                // Факторизация fi(num)
                List<ulong> factorization = new List<ulong>();
                ulong b, c;
                // Подсчёт количества элементов fiMas (Count не даёт верный результат из-за типа ulong)
                ulong fiMasCount = 0;
                foreach (ulong u in fiMas)
                {
                    fiMasCount++;
                }
                ulong temp = fiMasCount;
                while ((temp % 2) == 0)
                {
                    temp = temp / 2;
                    if (!factorization.Contains(2))
                        factorization.Add(2);
                }
                b = 3;
                c = (ulong)Math.Sqrt(temp) + 1;
                while (b < c)
                {
                    if ((temp % b) == 0)
                    {
                        if (temp / b * b - temp == 0)
                        {
                            if (!factorization.Contains(b))
                                factorization.Add(b);
                            temp = temp / b;
                            c = (ulong)Math.Sqrt(temp) + 1;
                        }
                        else
                            b += 2;
                    }
                    else
                        b += 2;
                }
                if (!factorization.Contains(temp))
                    factorization.Add(temp);
                // Проверка каждоого основания [2...num-1]
                List<ulong> roots = new List<ulong>();
                for (ulong i = 2; i < num; i++)
                {
                    bool check = true;
                    foreach (ulong a in factorization)
                    {
                        if (Power(i, fiMasCount / a, num) % num == 1)
                        {
                            check = false;
                            break;
                        }
                    }
                    if (check)
                        roots.Add(i);
                }
                // Вывод значений
                DateTime EndTime = DateTime.Now; // время окончания поиска корней
                label4.Text = (EndTime - StartTime).ToString();
                ulong j = 1;
                dataGridView1.Rows.Clear();
                foreach (ulong u in roots)
                {
                    dataGridView1.Rows.Add();
                    dataGridView1[0, dataGridView1.Rows.Count - 1].Value = j;
                    dataGridView1[1, dataGridView1.Rows.Count - 1].Value = u;
                    j++;
                }
                return 0;
            }
            
        }

        // Вычисление первообразного корня (1ое задание)
        private void button1_Click(object sender, EventArgs e)
        {
            ulong num = 0;

            // Ввод числа в необсходимой системе счисления
            if (radioButton1.Checked)
            {
                try
                {
                    num = Convert.ToUInt64(textBox1.Text, 10);
                }
                catch
                {
                    MessageBox.Show("Введено некорректное число!");
                    return;
                }
            }
            else
            {
                if (radioButton2.Checked)
                {
                    try
                    {
                        num = Convert.ToUInt64(textBox1.Text, 16);
                    }
                    catch
                    {
                        MessageBox.Show("Введено некорректное число!");
                        return;
                    }
                }
                else
                {
                    try
                    {
                        num = Convert.ToUInt64(textBox1.Text, 2);
                    }
                    catch
                    {
                        MessageBox.Show("Введено некорректное число!");
                        return;
                    }
                }
            }

            if (num == 0) // Если число равно 0, то первообразных кореней нет
            {
                dataGridView1.Rows.Clear();
                label4.Text = "";
                return;
            }
            if (num == 2) // Если число равно 2, то первообразный корень равен 1
            {
                dataGridView1.Rows.Clear();
                label4.Text = "";
                dataGridView1.Rows.Add();
                dataGridView1[0, dataGridView1.Rows.Count - 1].Value = 1;
                dataGridView1[1, dataGridView1.Rows.Count - 1].Value = 1;
                return;
            }


            double alpha = 1;
            
            while(true)
            {
                double root = Math.Pow(num, 1.0/alpha);

                if (root <= 2) // процесс останавливается, корней нет
                {
                    dataGridView1.Rows.Clear();
                    label4.Text = "";
                    return;
                }
                if((root % 1 == 0) && CheckSimple(root)) // если число простое - корни есть
                {
                    FindPrimitiveRoot(num,false);
                    return;
                }
                else // иначе делим число на 2 и снова пытаемся проверить
                {
                    ulong tempNumb = num / 2;

                    root = Math.Pow(tempNumb, 1.0 / alpha);
                    if (root <= 2) // процесс останавливается, корней нет
                    {
                        dataGridView1.Rows.Clear();
                        label4.Text = "";
                        return;
                    }
                    if ((root % 1 == 0) && CheckSimple(root)) // если число простое - корни есть
                    {
                        FindPrimitiveRoot(num, false);
                        return;
                    }
                    alpha++; // если нет, то увеличиваем alpha
                }
            }
        }

        // Выбор ввода n
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            // Ввод вручную
            if(radioButton4.Checked)
            {
                n = 0;
                label6.Text = "";
                textBox2.Enabled = true; // тестовое поле ввода n
                textBox2.Text = "";
                button3.Enabled = true; // кнопка подтверждения ввода n
                button2.Enabled = false; // кнопка генерации n
            }
            // Ввод автоматически
            else
            {
                n = 0;
                label6.Text = "";
                textBox2.Enabled = false; // тестовое поле ввода n
                textBox2.Text = "";
                button3.Enabled = false; // кнопка подтверждения ввода n
                button2.Enabled = true; // кнопка генерации Xa
            }
        }
        
        // Генерация простого большого числа
        private ulong GenerateBigSimple()
        {
            while (true)
            {
                bool check = false;
                var r = new Random(Guid.NewGuid().GetHashCode());
                var b = new byte[sizeof(ulong)];
                r.NextBytes(b);
                var number = BitConverter.ToUInt64(b, 0);
                number |= 1; // для того, чтобы число не было чётным
                // Проверяем делится ли оно на первые 500 простых чисел
                for (int i = 0; i < simple_numbers.Length; i++)
                {
                    if (number % (ulong) simple_numbers[i] == 0) // если да - полученное число не простое, повтор
                    {
                        check = true;
                        break;
                    }
                }
                // если успешно пройдена преыдущая проверка, то проводим тест рабина миллера 5 араз
                if (!check)
                {
                    if (RMGenerate(number))
                    {
                        return number;
                    }
                }
            }
        }

        // Генерация n
        private void button2_Click(object sender, EventArgs e)
        {
            n = GenerateBigSimple();
            label6.Text = n.ToString();
        }

        // Проверка введённого просто большого числа пользователем на простоту
        private bool CheckUserSimple(ulong number)
        {
            if (number % 2 == 0) return false; // если чётное - значит оно не простое
            // Проверяем делится ли оно на первые 500 простых чисел
            for (int i = 0; i < simple_numbers.Length; i++)
            {
                if (number % simple_numbers[i] == 0) return false;// если да - полученное число не простое, повтор
            }
            // если успешно пройдена преыдущая проверка, то проводим тест рабина миллера 5 араз
            if (RMGenerate(number)) return true;
            return false;
        }

        // Ввод n пользователем
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                n = Convert.ToUInt64(textBox2.Text);
            }
            catch
            {
                MessageBox.Show("Введено некорректное число!");
                n = 0;
                return;
            }
            if (n < Math.Pow(2, 64))
            {
                MessageBox.Show("n должно быть больше 9223372036854775807");
                n = 0;
                return;
            }
            // Проверка введённого числа на простоту
            if (!CheckUserSimple(n))
            {
                n = 0;
                MessageBox.Show("Ошибка! n не является простым!");
                return;
            }
            label6.Text = n.ToString();
        }

        // Тест Рабина-Миллера для полученного простого числа
        private bool RMGenerate(BigInteger n)
        {
            int s = 0;
            BigInteger r = n - 1;

            while (r % 2 == 0)
            {
                s += 1;
                r /= 2;
            }
            for (int i = 0; i < 5; i++)
            {
                BigInteger a;
                byte[] _a = new byte[n.ToByteArray().LongLength];
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                do
                {
                    rng.GetBytes(_a);
                    a = new BigInteger(_a);
                }
                while (a < 2 || a >= n - 2);
                BigInteger y = BigInteger.ModPow(a, r, n);
                if ((y == 1) || (y == n - 1))
                {
                    continue;
                }
                for (int j = 1; j < s; j++)
                {
                    y = (y * y) % n;
                    if (y == 1)
                    {
                        return false;
                    }
                    if (y == (n - 1))
                        break;
                }
                if (y != (n - 1))
                {
                    return false;
                }
            }
            return true;
        }

        // Ввод личного ключа Алисы пользователем
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                xa = Convert.ToUInt64(textBox3.Text);
            }
            catch
            {
                MessageBox.Show("Введено некорректное число!");
                xa = 0;
                return;
            }
            if (xa < Math.Pow(2, 64))
            {
                MessageBox.Show("Xa должно быть больше 9223372036854775807");
                xa = 0;
                return;
            }
            // Проверка введённого числа на простоту
            if (!CheckUserSimple(xa))
            {
                xa = 0;
                MessageBox.Show("Ошибка! Xa не является простым!");
                return;
            }
            label7.Text = n.ToString();
        }

        // Генерация Xa
        private void button5_Click(object sender, EventArgs e)
        {
            xa = GenerateBigSimple();
            label7.Text = xa.ToString();
        }

        // Выбор ввода Xa
        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            // Ввод вручную
            if (radioButton7.Checked)
            {
                xa = 0;
                label7.Text = "";
                textBox3.Enabled = true; // тестовое поле ввода Xa
                textBox3.Text = "";
                button4.Enabled = true; // кнопка подтверждения ввода Xa
                button5.Enabled = false; // кнопка генерации Xa
            }
            // Ввод автоматически
            else
            {
                xa = 0;
                label7.Text = "";
                textBox3.Enabled = false; // тестовое поле ввода Xa
                textBox3.Text = "";
                button4.Enabled = false; // кнопка подтверждения ввода Xa
                button5.Enabled = true; // кнопка генерации Xa
            }
        }

        // Генерация Xb
        private void button7_Click(object sender, EventArgs e)
        {
            xb = GenerateBigSimple();
            label9.Text = xb.ToString();
        }

        // Ввод личного ключа Боба пользователем
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                xb = Convert.ToUInt64(textBox3.Text);
            }
            catch
            {
                MessageBox.Show("Введено некорректное число!");
                xb = 0;
                return;
            }
            if (xb < Math.Pow(2, 64))
            {
                MessageBox.Show("Xb должно быть больше 9223372036854775807");
                xb = 0;
                return;
            }
            // Проверка введённого числа на простоту
            if (!CheckUserSimple(xa))
            {
                xb = 0;
                MessageBox.Show("Ошибка! Xb не является простым!");
                return;
            }
            label9.Text = n.ToString();
        }

        // Выбор ввода Xb
        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            // Ввод вручную
            if (radioButton9.Checked)
            {
                xb = 0;
                label9.Text = "";
                textBox4.Enabled = true; // тестовое поле ввода Xb
                textBox4.Text = "";
                button6.Enabled = true; // кнопка подтверждения ввода Xb
                button7.Enabled = false; // кнопка генерации Xb
            }
            // Ввод автоматически
            else
            {
                xb = 0;
                label9.Text = "";
                textBox4.Enabled = false; // тестовое поле ввода Xb
                textBox4.Text = "";
                button6.Enabled = false; // кнопка подтверждения ввода Xb
                button7.Enabled = true; // кнопка генерации Xb
            }
        }

        // Очистка label в схеме обмена ключами
        private void ClearLabels()
        {
            label12.Text = "";
            label14.Text = "";
            label16.Text = "";
            label18.Text = "";
            label20.Text = "";
            label29.Text = "";
            label27.Text = "";
            label23.Text = "";
            label22.Text = "";
            label37.Text = "";
            label35.Text = "";
            label33.Text = "";
            label31.Text = "";
            label25.Text = "";
        }

        // Если переключились на другую вкладку
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
                groupBox4.Enabled = false;
            else
                groupBox4.Enabled = true;
        }

        // Начало обмена ключами
        private void button8_Click(object sender, EventArgs e)
        {
            ulong g; // первообразный корень
            ulong ka; //секретный общий ключ (у алисы)
            ulong kb; //секретный общий ключ (у боба)

            ClearLabels(); // очиста Label в схеме обмена ключами
            if ((n == 0) || (xa == 0) || (xb == 0))
            {
                MessageBox.Show("Ошибка! Введены не все числа!");
                return;
            }
            g = FindPrimitiveRoot(n, true);
            ya = Power(g, xa, n) % n;
            yb = Power(g, xb, n) % n;
            ka = Power(yb, xa, n) % n;
            kb = Power(ya, xb, n) % n;
            label20.Text = ka.ToString();
            label25.Text = kb.ToString();
            label18.Text = ya.ToString();
            label23.Text = ya.ToString();
            label22.Text = yb.ToString();
            label31.Text = yb.ToString();
            label14.Text = g.ToString();
            label27.Text = g.ToString();
            label35.Text = g.ToString();
            label16.Text = xa.ToString();
            label33.Text = xb.ToString();
            label12.Text = n.ToString();
            label29.Text = n.ToString();
            label37.Text = n.ToString();
        }
    }
}


