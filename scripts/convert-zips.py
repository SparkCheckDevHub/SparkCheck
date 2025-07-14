# This Python script generates the db/zipcodes.sql file based on the list of US
# zip codes from https://github.com/millbj92/US-Zip-Codes-JSON
import requests

# Download the list of US zip codes
zip_codes = requests.get(
    "https://raw.githubusercontent.com/millbj92/US-Zip-Codes-JSON/refs/heads/master/USCities.json"
).json()

# Define maps for states (+ territories) and cities
state_index = 1
city_index = 1
states = {}
cities = {}

# Define state code to state name translations
state_names = {
    "AL": "Alabama", "AK": "Alaska", "AZ": "Arizona", "AR": "Arkansas",
    "CA": "California", "CO": "Colorado", "CT": "Connecticut", "DE": "Delaware",
    "FL": "Florida", "GA": "Georgia", "HI": "Hawaii", "ID": "Idaho",
    "IL": "Illinois", "IN": "Indiana", "IA": "Iowa", "KS": "Kansas",
    "KY": "Kentucky", "LA": "Louisiana", "ME": "Maine", "MD": "Maryland",
    "MA": "Massachusetts", "MI": "Michigan", "MN": "Minnesota", "MS": "Mississippi",
    "MO": "Missouri", "MT": "Montana", "NE": "Nebraska", "NV": "Nevada",
    "NH": "New Hampshire", "NJ": "New Jersey", "NM": "New Mexico", "NY": "New York",
    "NC": "North Carolina", "ND": "North Dakota", "OH": "Ohio", "OK": "Oklahoma",
    "OR": "Oregon", "PA": "Pennsylvania", "RI": "Rhode Island", "SC": "South Carolina",
    "SD": "South Dakota", "TN": "Tennessee", "TX": "Texas", "UT": "Utah",
    "VT": "Vermont", "VA": "Virginia", "WA": "Washington", "WV": "West Virginia",
    "WI": "Wisconsin", "WY": "Wyoming", "DC": "District of Columbia", "AS": "American Samoa",
    "GU": "Guam", "MP": "Northern Mariana Islands", "PR": "Puerto Rico",
    "VI": "U.S. Virgin Islands", "UM": "U.S. Minor Outlying Islands",
    "AE": "U.S. Armed Forces - Europe", "AP": "U.S. Armed Forces - Pacific",
    "AA": "U.S. Armed Forces - Americas", "PW": "Palau", "CZ": "Panama Canal Zone",
    "PI": "Philippine Islands", "TT": "Trust Territory of the Pacific Islands",
    "FM": "Micronesia", "MH": "Marshall Islands"
}

# Ensure state is added to TStates
def ensure_state(f, state_code):
    global state_index
    if state_code not in states:
        states[state_code] = state_index
        state_name = state_names.get(state_code, "Unknown")
        state_name = state_name.replace("'", "''")
        f.write(
            f"INSERT INTO TStates ( intStateID, strStateCode, strState ) VALUES ( {state_index}, '{state_code}', '{state_name}' )\n"
        )
        state_index += 1
    return states[state_code]

# Ensure city is added to TCities
def ensure_city(f, state_id, city):
    global city_index
    if state_id not in cities:
        cities[state_id] = {}
    if city not in cities[state_id]:
        cities[state_id][city] = city_index
        city_escaped = city.replace("'", "''")
        f.write(
            f"INSERT INTO TCities ( intCityID, strCity, intStateID ) VALUES ( {city_index}, '{city_escaped}', {state_id} )\n"
        )
        city_index += 1
    return cities[state_id][city]

# Generate the db/zipcodes.sql file
with open("../db/zipcodes.sql", "w", encoding="utf-8") as f:
    # Write header comment and options for SQL file
    f.write('''-- --------------------------------------------------------------------------------
-- Name: Auto-Generated (https://github.com/millbj92/US-Zip-Codes-JSON)
-- Project: SparkCheck
-- Abstract: SparkCheck ZipCodes list
-- --------------------------------------------------------------------------------

-- --------------------------------------------------------------------------------
-- Options
-- --------------------------------------------------------------------------------

-- Wait for DB to be created
WHILE DB_ID('dbSparkCheck') IS NULL
BEGIN
    PRINT 'Waiting for database dbSparkCheck...'
    WAITFOR DELAY '00:00:05'
END

-- Switch to DB
USE dbSparkCheck;
GO

SET NOCOUNT ON;
GO

-- Wait for relevant tables to be created
WHILE OBJECT_ID('dbo.TCities', 'U') IS NULL
   OR OBJECT_ID('dbo.TStates', 'U') IS NULL
   OR OBJECT_ID('dbo.TZipCodes', 'U') IS NULL
BEGIN
    PRINT 'Waiting for tables...'
	WAITFOR DELAY '00:00:05'
END

-- Exit if tables are already populated
IF EXISTS (SELECT 1 FROM dbo.TCities)
    THROW 50000, 'TCities table already populated', 1;

IF EXISTS (SELECT 1 FROM dbo.TStates)
    THROW 50001, 'TStates table already populated', 1;

IF EXISTS (SELECT 1 FROM dbo.TZipCodes)
    THROW 50002, 'TZipCodes table already populated', 1;

-- --------------------------------------------------------------------------------
-- Insert Values
-- --------------------------------------------------------------------------------
''')

    # Loop through entire list of zip codes to format as SQL
    for i, zip_code in enumerate(zip_codes):
        state_id = ensure_state(f, zip_code["state"])
        city_id = ensure_city(f, state_id, zip_code["city"])
        lat = float(zip_code.get('latitude') or 0)
        lon = float(zip_code.get('longitude') or 0)
        f.write(
            f"INSERT INTO TZipCodes ( strZipCode, intCityID, decLatitude, decLongitude ) VALUES ( '{zip_code['zip_code']}', {city_id}, {lat}, {lon} )\n"
        )
        if (i + 1) % 1000 == 0:
            f.write("GO\n")

print("Done generating zipcodes.sql")
