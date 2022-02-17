using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Great_ObalekeO
{
    class Program{
        static SqlDataReader dr;

        private static String connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=U:\\databases\\Great_ObalekeO.mdf;Integrated Security=True";


       static string strEmployeeID = "Id";
       static string strFirstName = "FirstName";
       static string strLastName = "LastName";
       static  string jobTitle = "JobTitle";
       static string dateOfBirth = "DOB";
       static string hireDate = "HireDate";
       static string monthlySalary = "MonthlySalary";
       static string salesAmount = "SalesAmount";
       static string bonusRate = "BonusRate";
       static string carAllowance = "CarAllowance";
        static string bonusAmount = "BonusAmount";
        static string totalComp = "TotalComp";

        static void Main(string[] args){


            Console.WriteLine("SORTED BY JOB TITLE THEN MONTHLY SALARY");
            SelectRows();

            UpdateRows();

            Console.WriteLine("\nSORTED BY JOB TITLE THEN TOTAL COMPENSATION");
            OrderByJobTitleTotalComp();

            calculateTotal();
        }

        private static void SelectRows()
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    Console.WriteLine("Select Rows:: Connection Opened");

                    // Sql Select Query    
                    string sql = "SELECT * FROM Employees ORDER BY JobTitle ASC, MonthlySalary";
                    SqlCommand cmd = new SqlCommand(sql, sqlConnection);

                    dr = cmd.ExecuteReader();


                    Console.WriteLine("{0} | {1} | {2} | {3} | {4} | {5} | ${6} | ${7} | {8} | {9} | ${10} | ${11} |", strEmployeeID.PadRight(8), strFirstName.PadRight(10), strLastName.PadRight(10), 
                        jobTitle.PadRight(20), dateOfBirth.PadRight(10), hireDate.PadRight(10), monthlySalary.PadRight(10), salesAmount.PadRight(10), bonusRate.PadRight(10), carAllowance.PadRight(10),
                        bonusAmount.PadRight(10), totalComp);
                    
                    Console.WriteLine("=======================================================================================");

                   

                    while (dr.Read())
                    {
                        DateTime dob = dr.GetDateTime(dr.GetOrdinal("DOB"));
                        DateTime hireDate = dr.GetDateTime(dr.GetOrdinal("hireDate"));

                        //reading from the datareader  
                        Console.WriteLine("{0} | {1} | {2} | {3} | {4} | {5} | {6} | {7} | {8} | {9} | {10} | {11} |",
                           dr["Id"].ToString().PadRight(8),
                           dr["FirstName"].ToString().PadRight(10),
                           dr["LastName"].ToString().PadRight(10),
                           dr["JobTitle"].ToString().PadRight(20),
                           dob.ToString("dd/MM/yyyy").PadLeft(10).PadRight(10),
                           hireDate.ToString("dd/MM/yyyy").PadRight(10),
                           dr["MonthlySalary"].ToString().PadRight(14),
                           dr["SalesAmount"].ToString().PadRight(10),
                           dr["BonusRate"].ToString().PadRight(10),
                           dr["CarAllowance"].ToString().PadRight(14),
                           dr["BonusAmount"].ToString().PadRight(10),
                           dr["TotalCompensation"]
                           );
                    }
                    dr.Close();

                    Console.WriteLine("=======================================================================================");
                    sqlConnection.Close();
                }
            }
            catch (SqlException ex)
            {
                // Display error         
                Console.WriteLine("Error: " +
                    ex.ToString());
            }
        }



        private static void UpdateRows(){
            try{

                using (SqlConnection sqlConnection = new SqlConnection(connectionString)){
                    sqlConnection.Open();

                    //calculate bonus amount and total compensation for each
                    int presidentBonusAmount = 0;
                    int salesAssociateBonus = 0;

                    int totalSalesAmount = 0;
                    int salesManagerBonus = 0;

                    int totalCompensation = 0;

                    string sql = "SELECT * FROM Employees";
                    SqlCommand cmd = new SqlCommand(sql, sqlConnection);

                    dr = cmd.ExecuteReader();

                    //Calculate bonuses
                    while (dr.Read()){
                        totalSalesAmount += (Int32.Parse(dr["SalesAmount"].ToString()));
                    }

                    dr.Close();
                    

                    //Calculate bonus for president
                    String strOne = "SELECT * FROM Employees WHERE JobTitle = 'President' ";
                    SqlCommand cmdGetPresident = new SqlCommand(strOne, sqlConnection);
                    dr = cmdGetPresident.ExecuteReader();

                    while (dr.Read())
                    {
                        presidentBonusAmount = (int)(dr.GetInt32(dr.GetOrdinal("MonthlySalary")) * dr.GetDecimal(dr.GetOrdinal("BonusRate")));
                    }

                    dr.Close();
                    //calculate bonus for president


                    //Calculate bonus for SA's
                    String strTwo = "SELECT * FROM Employees WHERE JobTitle = 'Sales Associate' ";
                    SqlCommand cmdGetSas = new SqlCommand(strTwo, sqlConnection);
                    List<Int32> saBonuses = new List<int>();
                    dr = cmdGetSas.ExecuteReader();

                    while (dr.Read())
                    {
                        salesAssociateBonus = (int)(dr.GetInt32(dr.GetOrdinal("SalesAmount")) * dr.GetDecimal(dr.GetOrdinal("BonusRate")));
                        saBonuses.Add(salesAssociateBonus);
                    }

                    dr.Close();
                    //calculate bonus for SA's 


                    //Calculate bonus for sales manager
                    String str = "SELECT * FROM Employees WHERE JobTitle = 'Sale Manager' ";
                    SqlCommand cmdGetSalesManager = new SqlCommand(str, sqlConnection);
                    dr = cmdGetSalesManager.ExecuteReader();

                    while (dr.Read())
                    {
                        salesManagerBonus = (int)(totalSalesAmount * dr.GetDecimal(dr.GetOrdinal("BonusRate")));
                    }

                    dr.Close();
                    //calculate bonus for sales manager
                   
                    Console.WriteLine("Update Rows");

                   

                    // Sql Update Statement- update bonus amounts   
                    StringBuilder updateSql = new StringBuilder();
                    updateSql.Append("UPDATE \n");
                    updateSql.Append("   Employees \n");
                    updateSql.Append("SET \n");
                    updateSql.Append("   BonusAmount = \n");
                    updateSql.Append("   CASE \n");
                    updateSql.Append("      when \n");
                    updateSql.Append("         JobTitle = 'President' \n");
                    updateSql.Append("      then \n");
                    updateSql.Append($"         {presidentBonusAmount} \n");

                    //
                    updateSql.Append("      when \n");
                    updateSql.Append($"         JobTitle = 'Sales Associate' AND Id = {202} \n");
                    updateSql.Append("      then \n");
                    updateSql.Append($"         {saBonuses[0]} \n");
                    updateSql.Append("      when \n");
                    updateSql.Append($"       JobTitle = 'Sales Associate' AND  Id  = {203} \n");
                    updateSql.Append("      then \n");
                    updateSql.Append($"         {saBonuses[1]} \n");
                    updateSql.Append("      when \n");
                    updateSql.Append($"       JobTitle = 'Sales Associate' AND  Id  = {297} \n");
                    updateSql.Append("      then \n");
                    updateSql.Append($"         {saBonuses[2]} \n");
                    //

                    updateSql.Append("      when \n");
                    updateSql.Append("         JobTitle = 'Sale Manager' \n");
                    updateSql.Append("      then \n");
                    updateSql.Append($"         {salesManagerBonus} \n");
                    updateSql.Append("      when \n");
                    updateSql.Append("         JobTitle = 'Programmer' \n");
                    updateSql.Append("      then \n");
                    updateSql.Append($"         {0} \n");
                    updateSql.Append("      when \n");
                    updateSql.Append("         JobTitle = 'Programmer Associate' \n");
                    updateSql.Append("      then \n");
                    updateSql.Append($"         {0} \n");
                    updateSql.Append("   END \n");
                    updateSql.Append("WHERE \n");
                    updateSql.Append("   JobTitle IN \n");
                    updateSql.Append("   ( \n");
                    updateSql.Append("      'President', 'Sales Associate', 'Sale Manager', 'Programmer', 'Programmer Associate' \n");
                    updateSql.Append("   ) \n");
                    updateSql.Append(";");


                    SqlCommand UpdateCmd = new SqlCommand(updateSql.ToString(), sqlConnection);
                    UpdateCmd.ExecuteNonQuery();


                    //Update total compensation after updating bonus amount
                    dr = cmd.ExecuteReader();

                    Console.WriteLine("Total Compensations\n");

                    List<Int32> comps = new List<int>();
                    while (dr.Read())
                    {
                        totalCompensation += dr.GetInt32(dr.GetOrdinal("MonthlySalary")) + dr.GetInt32(dr.GetOrdinal("CarAllowance")) + dr.GetInt32(dr.GetOrdinal("BonusAmount"));
                        Console.WriteLine(totalCompensation.ToString());

                        comps.Add(totalCompensation);
                    }
                   

                    dr.Close();


                    StringBuilder updateThree = new StringBuilder();
                    updateThree.Append("UPDATE \n");
                    updateThree.Append("   Employees \n");
                    updateThree.Append("SET \n");
                    updateThree.Append("   TotalCompensation = \n");
                    updateThree.Append("   CASE \n");
                    updateThree.Append("      when \n");
                    updateThree.Append($"         Id = {101}  \n");
                    updateThree.Append("      then \n");
                    updateThree.Append($"         {comps[0]} \n");
                    updateThree.Append("      when \n");
                    updateThree.Append($"         Id = {102} \n");
                    updateThree.Append("      then \n");
                    updateThree.Append($"         {comps[1]} \n");
                    updateThree.Append("      when \n");
                    updateThree.Append($"         Id = {103} \n");
                    updateThree.Append("      then \n");
                    updateThree.Append($"         {comps[2]} \n");
                    updateThree.Append("      when \n");
                    updateThree.Append($"         Id = {202} \n");
                    updateThree.Append("      then \n");
                    updateThree.Append($"         {comps[3]} \n");
                    updateThree.Append("      when \n");
                    updateThree.Append($"        Id = {203} \n");
                    updateThree.Append("      then \n");
                    updateThree.Append($"         {comps[4]} \n");
                    updateThree.Append("      when \n");
                    updateThree.Append($"        Id = {211} \n");
                    updateThree.Append("      then \n");
                    updateThree.Append($"         {comps[5]} \n");
                    updateThree.Append("      when \n");
                    updateThree.Append($"        Id = {297} \n");
                    updateThree.Append("      then \n");
                    updateThree.Append($"         {comps[5]} \n");
                    updateThree.Append("   END \n");
                    updateSql.Append("WHERE \n");
                    updateSql.Append("   Id IN \n");
                    updateSql.Append("   ( \n");
                    updateSql.Append($"      {101}, {102}, {103}, {202}, {203}, {211}, {297} \n");
                    updateSql.Append("   ) \n");
                    updateSql.Append(";");


                    SqlCommand UpdateCmdTwo = new SqlCommand(updateThree.ToString(), sqlConnection);
                    UpdateCmdTwo.ExecuteNonQuery();
                    //Update total compensation after updating bonus amounts

                    sqlConnection.Close();
                }
            }

            catch (SqlException ex)
            {
                // Display error   
                Console.WriteLine("Error: " + ex.ToString());
            }
        }

        private static void calculateTotal() {
            String cmdOne = "SELECT SUM(CarAllowance) From Employees";
            String cmdTwo = "SELECT SUM(MonthlySalary) From Employees";
            String cmdThree = "SELECT SUM(SalesAmount) From Employees";
            String cmdFour = "SELECT SUM(TotalCompensation) From Employees";


            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {

                
                sqlConnection.Open();
                Console.WriteLine("Calculate Grand Totals");
                SqlCommand Cmd = new SqlCommand(cmdOne, sqlConnection);
                SqlCommand CmdTwo = new SqlCommand(cmdTwo, sqlConnection);
                SqlCommand CmdThree = new SqlCommand(cmdThree, sqlConnection);
                SqlCommand CmdFour = new SqlCommand(cmdFour, sqlConnection);

                int totalMonthlySalary = 0;
                int totalCompensation = 0;
                int totalCarAllowance = 0;
                int totalSalesAmt = 0;

                totalCarAllowance = Convert.ToInt32 (Cmd.ExecuteScalar());
                totalMonthlySalary = Convert.ToInt32(CmdTwo.ExecuteScalar());
                totalSalesAmt = Convert.ToInt32(CmdThree.ExecuteScalar());
                totalCompensation = Convert.ToInt32(CmdFour.ExecuteScalar());


                sqlConnection.Close();
                Console.WriteLine("Grand Totals\n");
                Console.WriteLine("MonthlySalary::${0}  Compensation::${1}  Sales Amount::${2}  Car allowance::${3}",
                    totalMonthlySalary.ToString("N"), 
                   totalCompensation.ToString("N"),
                    totalSalesAmt.ToString("N"), 
                    totalCarAllowance.ToString("N"));
                
            }

        }

        private static void OrderByJobTitleTotalComp()
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();

                    // Sql Select Query    
                    string sql = "SELECT * FROM Employees ORDER BY JobTitle ASC, TotalCompensation ";
                    SqlCommand cmd = new SqlCommand(sql, sqlConnection);

                    dr = cmd.ExecuteReader();


                    Console.WriteLine("{0} | {1} | {2} | {3} | {4} | {5} | ${6} | ${7} | {8} | {9} | ${10} | ${11} |", strEmployeeID.PadRight(10), strFirstName.PadRight(10), strLastName.PadRight(10),
                        jobTitle.PadRight(20), dateOfBirth.PadRight(10), hireDate.PadRight(10), monthlySalary.PadRight(10), salesAmount.PadRight(10), bonusRate.PadRight(10), 
                        carAllowance.PadRight(10), bonusAmount.PadRight(10), totalComp);

                    Console.WriteLine("==================================================================================================");
                    while (dr.Read())
                    {
                        DateTime dob = dr.GetDateTime(dr.GetOrdinal("DOB"));
                        DateTime hireDate = dr.GetDateTime(dr.GetOrdinal("hireDate"));

                        //reading from the datareader  
                        Console.WriteLine("{0} | {1} | {2} | {3} | {4} | {5} | {6} | {7} | {8} | {9} | {10} | {11} |",
                           dr["Id"].ToString().PadRight(8),
                           dr["FirstName"].ToString().PadRight(10),
                           dr["LastName"].ToString().PadRight(10),
                           dr["JobTitle"].ToString().PadRight(20),
                           dob.ToString("dd/MM/yyyy").PadLeft(10).PadRight(10),
                           hireDate.ToString("dd/MM/yyyy").PadRight(10),
                           dr["MonthlySalary"].ToString().PadRight(14),
                           dr["SalesAmount"].ToString().PadRight(10),
                           dr["BonusRate"].ToString().PadRight(10),
                           dr["CarAllowance"].ToString().PadRight(14),
                           dr["BonusAmount"].ToString().PadRight(10),
                           dr["TotalCompensation"]
                           );
                    }
                    dr.Close();

                    Console.WriteLine("==========================================");
                    sqlConnection.Close();
                }
            }
            catch (SqlException ex)
            {
                // Display error         
                Console.WriteLine("Error: " +
                    ex.ToString());
            }
        }


        public override string ToString()
        {
            return base.ToString();
        }

    }
}
