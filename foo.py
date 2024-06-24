import duckdb

df = duckdb.sql("""
SELECT text from 'data/*.json' 
where location = 'WWT Slimbridge'
and lower(text) like '%van de%'
""").df()

for t in df["Text"]:
    print(t)