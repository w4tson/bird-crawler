import duckdb

duckdb.sql("""
SELECT location, count(*) c from 'datapoints-*2024.json'
group by location
order by c desc  
""").show()


