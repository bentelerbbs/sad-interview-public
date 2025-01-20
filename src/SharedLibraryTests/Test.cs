using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibraryTests
{
    public class Test
    {
        public static void TryWaitForConditionToBeTrue(Func<bool> condition, string description, int tries = 180)
        {
            try
            {
                if (condition?.Invoke() == true)
                {
                    Console.WriteLine($"TryWaitForConditionToBeTrue did not even start cycle '{description}'");
                    return;
                }
                Console.WriteLine($"TryWaitForConditionToBeTrue START '{description}'");
                int i = 1;
                while (condition?.Invoke() == false)
                {
                    Console.WriteLine($"TryWaitForConditionToBeTrue waiting try ({i}/{tries})'");
                    i++;
                    if (i > tries)
                    {
                        break;
                    }
                    Thread.Sleep(1000);
                }
                if (condition?.Invoke() == true)
                {
                    Console.WriteLine($"TryWaitForConditionToBeTrue OK '{description}'");
                    return;
                }
                else
                {
                    Console.Error.WriteLine($"TryWaitForConditionToBeTrue NOT ok '{description}'");
                    Assert.Fail($"TryWaitForConditionToBeTrue NOT ok '{description}'");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                Assert.Fail($"TryWaitForConditionToBeTrue EXCEPTION: '{description}'");
            }
        }

        /// <summary>
        /// Adds timestamp
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message)
        {
            Console.WriteLine($"{DateTime.Now:dd.MM.yyyy HH:mm:ss.fff}: {message}");
        }

        /// <summary>
        /// Compare 2 objects w JSON, Assert.Fail called when they differ
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool Compare<T>(T expected, T actual, string name)
        {
            string expectedJson = JsonConvert.SerializeObject(expected);
            string actualJson = JsonConvert.SerializeObject(actual);
            if (expectedJson.Equals(actualJson))
            {
                Log($"[OK] TestResult - '{name}'");
                return true;
            }
            else
            {
                Log($"[ERROR] TestResult - '{name}' details below:");
                Log($"expectedJson: {expectedJson}");
                Log($"actualJson: {actualJson}");
                Assert.Fail($"Compare ({name}) FAILED");
                return false;
            }
        }

        public static void PrepareTestDirectory(string path)
        {
            Console.WriteLine($"PrepareTestDirectory '{path}'");
            DeleteDirectoryIfItExists(path);
            Directory.CreateDirectory(path);
            Assert.IsTrue(Directory.Exists(path), $"Directory '{path}' NOT created ok");
        }

        public static void DeleteDirectoryIfItExists(string path)
        {
            try
            {
                if (Directory.Exists(path) == false)
                {
                    return;
                }
                Directory.Delete(path, true);
                Assert.IsTrue(Directory.Exists(path) == false, $"DeleteDirectoryIfItExists failed, directory '{path}' still exists");
                Console.WriteLine($"DeleteDirectoryIfItExists OK, path: '{path}'");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                Assert.Fail($"DeleteDirectoryIfItExists failed, path: '{path}', Exception above");
            }
        }

        public static void AssertDirectoryExists(string path)
        {
            Assert.IsTrue(Directory.Exists(path), $"AssertDirectoryExists FAIL, path: '{path}'");
            Log($"AssertDirectoryExists OK, path: '{path}'");
        }

        public static void AssertDirectoryDoesNotExist(string path)
        {
            Assert.IsFalse(Directory.Exists(path), $"AssertDirectoryDoesNotExist FAIL, path: '{path}'");
            Log($"AssertDirectoryDoesNotExist OK, path: '{path}'");
        }

        public static void AssertFileDoesNotExist(string path)
        {
            Assert.IsFalse(File.Exists(path), $"AssertFileDoesNotExist FAIL, path: '{path}'");
            Log($"AssertFileDoesNotExist OK, path: '{path}'");
        }

        public static void AssertFileExists(string path)
        {
            Assert.IsTrue(File.Exists(path), $"AssertFileExists FAIL, path: '{path}'");
            Log($"AssertFileExists OK, path: '{path}'");
        }

        public static void CompareFileAndBytes(string expected, byte[] actual, string testName)
        {
            Log($"CompareFileAndBytes:");
            Log($"expected: {expected}");
            Log($"actual: byte[]");
            byte[] expectedOriginal = File.ReadAllBytes(expected);
            CompareBytes(expectedOriginal, actual, testName);
        }

        public static void CompareFiles(string expected, string actual, string testName)
        {
            Log($"CompareFiles:");
            Log($"expected: {expected}");
            Log($"actual: {actual}");
            byte[] expectedOriginal = File.ReadAllBytes(expected);
            byte[] actualOriginal = File.ReadAllBytes(actual);
            CompareBytes(expectedOriginal, actualOriginal, testName);
        }

        public static void CompareBytes(byte[] expected, byte[] actual, string testName)
        {
            bool bytesEqual = expected.SequenceEqual(actual);
            if (bytesEqual == false)
            {
                if (expected.Length < 3 * 1000 && actual.Length < 3 * 1000)
                {
                    Log($"Expected bytes: {expected.Length}");
                    Log(string.Join(", ", expected));
                    Log($"actual bytes: {actual.Length}");
                    Log(string.Join(", ", actual));
                }
                //
                int diffCount = 0;
                if (expected.Length == actual.Length)
                {
                    Log($"Byte arrays are same length, differences below:");
                    for (int i = 0; i < expected.Length; i++)
                    {
                        if (actual[i] != expected[i])
                        {
                            Log($"Index: {i}, actual: {actual[i]}, expected: {expected[i]}");
                            diffCount++;
                        }
                    }
                    Log($"diffCount: {diffCount}");
                }
            }
            Assert.IsTrue(bytesEqual, "Byte array is NOT same as expected");
            Log($"[{testName}]: byte array is same as expected - length ({expected.Length}/{actual.Length})");
        }
    }
}
