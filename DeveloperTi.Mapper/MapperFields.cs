using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace DeveloperTi.Mapper
{
    public class MapperFields<TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// Estou reutilizando o método principal Map(DataRow dataRow) que faz o parse de somente um objeto.
        /// Percorro todo o DataTable, selecionando linha a linha e convertendo para objeto
        /// </summary>
        /// <param name="dataTable">DataTable obtido na consulta</param>
        /// <returns>Retorno uma lista de objetos já convertida</returns>
        public IEnumerable<TEntity> MapAll<TSource>(IList<TSource> dataTable)
        {
            return dataTable.Cast<TEntity>().Select(Map).ToList();
        }

        public IEnumerable<TEntity> MapAll(DataTable dataTable)
        {
            return dataTable.Rows.Cast<DataRow>().Select(Map).ToList();
        }

        /// <summary>
        /// Faço uma varreruda na propriedade do objeto, verificando se contem MapField como atributo, 
        /// Se existir, crio uma lista desses campos que foram criados no objeto. 
        /// Ex: [MapFieldName("first_name", "name_user", "name")], retornará 3 supostos campos que existem no banco
        /// </summary>
        /// <param name="type">object ou classe que contenha a propriedade</param>
        /// <param name="propertyName">nome da propriedade que contenha o attibute</param>
        /// <returns>Retorna uma coleção de string, assinados pelo attribute</returns>
        private static IEnumerable<string> GetFieldNameFromAttribute(Type type, string propertyName)
        {
            var property = type.GetProperty(propertyName)
                               .GetCustomAttributes(false)
                               .FirstOrDefault(p => p.GetType().Name.Equals("MapFieldNameAttribute"));
            return property != null ? ((MapFieldNameAttribute)property).AttributeNames : new List<string>();
        }

        /// <summary>
        /// Fará o parse de uma linha completa do DataTable para um objeto 
        /// </summary>
        /// <param name="dataRow">DataRow obtido de uma DataTable</param>
        /// <returns>Retorna um objeto mapeado com os valores vindos do BD</returns>
        private TEntity Map<TSource>(TSource dataRow)
        {
            var properties = typeof(TEntity).GetProperties()
                                            .Where(x => x.GetCustomAttributes(typeof(MapFieldNameAttribute), true).Any())
                                            .ToList();
            var entity = new TEntity();
            foreach (var property in properties)
            {
                Mapper(typeof(TEntity), dataRow, property, entity);
            }
            return entity;
        }

        /// <summary>
        /// Pegará o valor do DataRow e atribuirá na classe que desejo fazer o parse
        /// </summary>
        /// <param name="type">object ou classe que contenha a propriedade</param>
        /// <param name="row">DataRow com valores do parse</param>
        /// <param name="propertyInfo">propriedade com attibute anotation</param>
        /// <param name="entity">objeto que vai receber o valor do datarow</param>
        private static void Mapper<T>(Type type, T row, PropertyInfo propertyInfo, object entity)
        {
            var isDataRow = row is DataRow;
            var fields = GetFieldNameFromAttribute(type, propertyInfo.Name);
            foreach (var field in fields)
            {
                if (string.IsNullOrEmpty(field) || row == null)
                    continue;
                if (isDataRow)
                    if(!(row as DataRow).Table.Columns.Contains(field) || (row as DataRow)[field] == DBNull.Value)
                    continue;

                if (isDataRow)
                    propertyInfo.SetValue(entity, (row as DataRow)[field], null);
                else
                    propertyInfo.SetValue(entity, row.GetType().GetRuntimeProperty(propertyInfo.Name).GetValue(row), null);
                
                break;
            }
        }

        //private static void Mapper(Type type, TSource row, PropertyInfo propertyInfo, object entity)
        //{
        //    var fields = GetFieldNameFromAttribute(type, propertyInfo.Name);
        //    foreach (var field in fields)
        //    {
        //        if (string.IsNullOrEmpty(field) || !row.Table.Columns.Contains(field))
        //            continue;
        //        if (row[field] == DBNull.Value)
        //            continue;
        //        propertyInfo.SetValue(entity, row[field], null);
        //        break;
        //    }
        //}
    }
}
