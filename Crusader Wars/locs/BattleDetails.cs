﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using Crusader_Wars.terrain;
using System.Runtime.CompilerServices;

namespace Crusader_Wars.locs
{
    static class BattleDetails
    {
        static string Name { get; set; }
        
        public static void SetBattleName(string a)
        {
            Name = a;
        }

        public static void ChangeBattleDetails(Player player, Enemy enemy)
        {

            EditButtonVersion();
            EditBattleTextDetails(player, enemy);
            EditCombatSidesDetails(player, enemy);
            EditTerrainImage();

        }

        private static void EditButtonVersion()
        {
            string version_path = @".\app_version.txt";
            string version = File.ReadAllText(version_path);
            version = Regex.Match(version, @"""(.+)""").Groups[1].Value;

            
            string original_buttonVersion_path = @".\battle files\text\db\tutorial_historical_battles_uied_component_texts.loc.tsv";
            string copy_path = @".\data\tutorial_historical_battles_uied_component_texts.loc.tsv";
            File.Copy(original_buttonVersion_path, copy_path);
            File.WriteAllText(copy_path, string.Empty);

            string new_data = "";
            using (FileStream btnVersion = File.Open(original_buttonVersion_path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(btnVersion))
            {
                string line = "";
                while (line != null && !reader.EndOfStream)
                {
                    line = reader.ReadLine();

                    if (line.Contains("uied_component_texts_localised_string_string_NewState_Text_3a000c"))
                    {
                        string new_version = $"uied_component_texts_localised_string_string_NewState_Text_3a000c\tCrusader Wars V{version}\ttrue";
                        new_data += new_version + "\n";
                        continue;
                    }

                    new_data += line + "\n";
                }

                reader.Close();
                btnVersion.Close();
            }

            File.WriteAllText(copy_path, new_data);
            if (File.Exists(original_buttonVersion_path)) File.Delete(original_buttonVersion_path);
            File.Move(copy_path, original_buttonVersion_path);
        }

        private static void EditBattleTextDetails(Player player, Enemy enemy)
        {
            string patreon_text = "Special thanks to our patreons for supporting the development of the mod: Jermaine, Kameron, Michael Nathan Chananja Klaassen, Kyra, Gav, Carl Enqvist, I Regret This Already, Oron Gabay, Kyle T David, Ryan Merklen, Chris Kelly, Kieran Britt & Galahad.";

            string original_battle_details_path = @".\battle files\text\db\tutorial_historical_battles.loc.tsv";
            string copy_path = @".\data\tutorial_historical_battles.loc.tsv";
            File.Copy(original_battle_details_path, copy_path);
            File.WriteAllText(copy_path, string.Empty);


            string new_data = "";
            using (FileStream battle_details_file = File.Open(original_battle_details_path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(battle_details_file))
            {
                string line = "";
                while (line != null && !reader.EndOfStream)
                {
                    line = reader.ReadLine();

                    //BATTLE NAME
                    if (line.Contains("battles_localised_name_tut_tutorial_battle"))
                    {
                        line = Regex.Replace(line, @"\t(?<BattleName>.+)\t", $"\t{Name}\t");
                    }

                    //BATTLE DETAILS
                    if (line.Contains("battles_description_tut_tutorial_battle"))
                    {
                        string player_side_realm_name = player.RealmName;
                        string enemy_side_realm_name = enemy.RealmName;

                        double player_total_soldiers = player.TotalNumber;
                        double enemy_total_soldiers = enemy.TotalNumber;

                        string new_text = $"{player_side_realm_name}" + "  VS  " + $"{enemy_side_realm_name}" +
                                           "\\\\n" +
                                          $"Total Soldiers: {player_total_soldiers}" + "\\\\t||\\\\t" + $"Total Soldiers: {enemy_total_soldiers}" +
                                          "\\\\n\\\\n" +
                                          $"{patreon_text}";

                        line = Regex.Replace(line, @"\t(?<BattleName>.+)\t", $"\t{new_text}\t");
                    }
                    new_data += line + "\n";
                }

                reader.Close();
                battle_details_file.Close();
            }

            File.WriteAllText(copy_path, new_data);
            if(File.Exists(original_battle_details_path))File.Delete(original_battle_details_path);
            File.Move(copy_path, original_battle_details_path);
        }

        private static void EditCombatSidesDetails(Player player, Enemy enemy)
        {
            string original_attila_file_path = @".\battle files\text\db\tutorial_historical_battles_factions.loc.tsv";
            string copy_path = @".\data\tutorial_historical_battles_factions.loc.tsv";
            File.Copy(original_attila_file_path, copy_path);
            File.WriteAllText(copy_path, string.Empty);

            string new_data = "";
            using (FileStream battle_side_file = File.Open(original_attila_file_path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(battle_side_file))
            {
                string line = "";
                while (line != null && !reader.EndOfStream)
                {
                    line = reader.ReadLine();

                    //Enemy Side
                    if (line.Contains("factions_screen_name_historical_house_bolton"))
                    {
                        line = Regex.Replace(line, @"\t(?<CombatSide>.+)\t", $"\t{enemy.CombatSide}\t");
                    }

                    //Player Side
                    if(line.Contains("factions_screen_name_historical_house_stark"))
                    {
                        line = Regex.Replace(line, @"\t(?<CombatSide>.+)\t", $"\t{player.CombatSide}\t");
                    }

                    new_data += line + "\n";
                }

                reader.Close();
                battle_side_file.Close();
            }

            File.WriteAllText(copy_path, new_data);
            if(File.Exists(original_attila_file_path))File.Delete(original_attila_file_path);
            File.Move(copy_path, original_attila_file_path);
        }

        private static void EditTerrainImage()
        {
            /*  SNOW VALUES
             * MildWinter
             * NormalWinter
             * HarshWinter
             * 
             * nullOrEmpty
             */

            /*  SEASONS VALUES
             * winter
             * fall
             * spring
             * summer
             * 
             * null value = random
             */

            
            string images_folder_path = @".\data\terrains_images\";

            string image_to_copy_path = "";

            
            string terrain = TerrainGenerator.TerrainType;
            string weather = Weather.Season;
            string snow = Weather.Winter_Severity;
            bool hasSnow = Weather.HasWinter;
            


            //For each terrain folder
            foreach (var folder_path in Directory.GetDirectories(images_folder_path))
            {
                string folder_name = Path.GetFileName(folder_path);

                if (terrain == folder_name)
                {
                    //For each image on folder
                    foreach(var image_path in Directory.GetFiles(folder_path))
                    {
                        string image_name = Path.GetFileNameWithoutExtension(image_path);
                        terrain = FirstCharSubstring(terrain);
                        

                        //Terrain Image
                        if(weather == "random" && image_name == terrain)
                        {
                            image_to_copy_path = image_path;
                            break;
                        }

                        //Terrain Image + Weather
                        if(weather != "random" && !hasSnow && image_name == $"{terrain}_{weather}")
                        {
                            image_to_copy_path = image_path;
                            break;
                        }
                        //Terrain Image + Weather + Snow
                        if (weather != "random" && hasSnow && image_name == $"{terrain}_{weather}_{GetSnow(snow)}")
                        {
                            image_to_copy_path = image_path;
                            break;
                        }

                    }

                    break;
                }
            }

            string default_image_path = @".\data\terrains_images\screenshot_small.png";
            
            //Default Version Image
            if(string.IsNullOrEmpty(image_to_copy_path))
            {
                image_to_copy_path = default_image_path;
            }

            string battle_files_image_path = @".\battle files\script\tut_tutorial_battle\screenshot_small.png";

            if(File.Exists(battle_files_image_path)) File.Delete(battle_files_image_path);
            File.Copy(image_to_copy_path, battle_files_image_path);


        }



        private static string FirstCharSubstring(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            if (input == "Desert Mountains") return "desert mountains";

            return $"{input[0].ToString().ToLower()}{input.Substring(1)}";
        }

        private static string GetSnow(string snow_severity)
        {
            switch(snow_severity)
            {
                case "MildWinter":
                    return "mildsnow";
                case "NormalWinter":
                    return "normalsnow";
                case "HarshWinter":
                    return "harshsnow";
                default:
                    return "";
            }
        }


    }
}
