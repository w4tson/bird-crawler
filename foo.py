import duckdb

duckdb.sql("""
SELECT text from 'data/*.json' 
where text like '%Bryant%'
""").show()